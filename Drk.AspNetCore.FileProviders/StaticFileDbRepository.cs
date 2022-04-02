using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Drk.AspNetCore.FileProviders
{
    public class StaticFileDbRepository
    {
        private readonly StaticFileDbContext dbctx;

        public StaticFileDbRepository(StaticFileDbContext dbContext)
        {
            this.dbctx = dbContext;
        }

        private void CreateParentDir(string path)
        {
            var p = path.Split('/');
            var que = new Queue<string>();
            for (int i = 1; i < p.Length; i++)
            {
                var parentDir = "/" + string.Join("/", que.ToArray());
                CreateDirectory(parentDir, true);
                que.Enqueue(p[i]);
            }
        }

        public void UpdateFile(string path, byte[] data, string userId, string clientIp)
        {
            using (var trn = dbctx.Database.BeginTransaction())
            {

                // create parent directory if not exists
                CreateParentDir(path);
                var dataEntity = new StaticFileData
                {
                    Path = path,
                    Content = data,
                    UserId = userId,
                    ClientIp = clientIp,
                    UpdateTime = DateTime.Now
                };
                dbctx.StaticFileDatas.Add(dataEntity);
                dbctx.SaveChanges(); //SaveChanges() to get inserted identity key
                var existing = dbctx.StaticFileIndices.FirstOrDefault(o => o.Path == path);
                if (existing == null)
                {
                    dbctx.StaticFileIndices.Add(new StaticFileIndex
                    {
                        Path = path,
                        FileDataId = dataEntity.FileDataId,
                        LastUpdate = dataEntity.UpdateTime,
                        Size = dataEntity.Content.Length
                    });
                }
                else
                {
                    var old = dbctx.StaticFileDatas.FirstOrDefault(o => o.FileDataId == existing.FileDataId);
                    if (old != null)
                    {
                        old.Status = "D";
                        old.Remark += GenRemark("Replace", userId, clientIp);
                    }
                    existing.FileDataId = dataEntity.FileDataId;
                    existing.LastUpdate = dataEntity.UpdateTime;
                    existing.Size = dataEntity.Content.Length;
                }
                dbctx.SaveChanges();
                trn.Commit();
            }
        }

        public IEnumerable<StaticFileIndex> GetFiles(string dir, bool recurse = false)
        {
            var all = dbctx.StaticFileIndices.Where(o => o.Path.StartsWith(dir) && o.Path != dir);
            if (!recurse)
            {
                return all.Where(o => !o.Path.Substring(dir.Length + 1).Contains("/")).ToArray();
            }
            return all.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns>return null when not found</returns>
        public StaticFileData ReadFile(string path)
        {
            var existing = dbctx.StaticFileIndices.SingleOrDefault(o => o.Path == path && o.FileDataId != Contstants.DirectoryIndex);
            if (existing == null) return null!;
            return dbctx.StaticFileDatas.SingleOrDefault(o => o.FileDataId == existing.FileDataId)!;
        }

        public void CreateDirectory(string path, bool ignoreExisting = false)
        {
            var existing = dbctx.StaticFileIndices.SingleOrDefault(o => o.Path == path && o.FileDataId == Contstants.DirectoryIndex);
            if (existing != null)
            {
                if (!ignoreExisting) throw new Exception($"Directory {path} already exists");
                return;
            }
            dbctx.StaticFileIndices.Add(new StaticFileIndex
            {
                Path = path,
                FileDataId = Contstants.DirectoryIndex,
                LastUpdate = DateTime.Now
            });
            dbctx.SaveChanges();
        }

        string GenRemark(string action, string userId, string clientIp) => $"{action} by {userId}[{clientIp}]@{DateTime.Now:yyyy-MM-dd HH:mm:ss}\n";

        public void DeleteFile(string path, string userId, string clientIp)
        {
            var existing = dbctx.StaticFileIndices.SingleOrDefault(o => o.Path == path && o.FileDataId != Contstants.DirectoryIndex);
            if (existing == null) throw new Exception($"File {path} not found");
            var dataEntity = dbctx.StaticFileDatas.SingleOrDefault(o => o.FileDataId == existing.FileDataId);
            if (dataEntity != null)
            {
                dataEntity.Status = "D";
                dataEntity.Remark += GenRemark("Deleted", userId, clientIp);
            }
            dbctx.StaticFileIndices.Remove(existing);
            dbctx.SaveChanges();
        }

        public void DeleteDirectory(string path, string userId, string clientIp)
        {
            using (var trn = dbctx.Database.BeginTransaction())
            {

                var existing = dbctx.StaticFileIndices.SingleOrDefault(o => o.Path == path && o.FileDataId == Contstants.DirectoryIndex);
                if (existing == null) throw new Exception($"Directory {path} not found");
                var children = dbctx.StaticFileIndices.Where(o => o.Path.StartsWith(path + "/")).ToList();
                var childrenFileDataIds = children.Where(o => !o.IsDirectory).Select(o => o.FileDataId).ToArray();
                var cn = dbctx.Database.GetDbConnection();
                cn.Execute(@"UPDATE StaticFileDatas SET Status='D',Remark=@remark WHERE FileDataId IN @childrenFileDataIds", new
                {
                    childrenFileDataIds,
                    remark = GenRemark("Delete Directory", userId, clientIp)
                }, transaction: trn.GetDbTransaction());
                dbctx.StaticFileIndices.RemoveRange(children);
                trn.Commit();
            }
        }
    }
}
