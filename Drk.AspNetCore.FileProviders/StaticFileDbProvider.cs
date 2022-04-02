using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Drk.AspNetCore.FileProviders
{
    public class StaticFileDbProvider : IFileProvider
    {
        private readonly IServiceProvider serviceProvider;

        IServiceScope CreateScope() => serviceProvider.CreateScope();

        public StaticFileDbProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;

        }
        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            using (var scope = CreateScope())
            {
                return new DirectoryContents(scope.ServiceProvider.GetRequiredService<StaticFileDbRepository>().GetFiles(subpath).ToArray());
            }
        }
        public IFileInfo GetFileInfo(string subpath)
        {
            using (var scope = CreateScope())
            {
                var find = scope.ServiceProvider.GetRequiredService<StaticFileDbRepository>().ReadFile(subpath);
                if (find != null) return new FileInfo(find);
                return new NotFoundFileInfo(subpath);
            }
        }
        public IChangeToken Watch(string filter) => NullChangeToken.Singleton;
    }

    public class DirectoryContents : IDirectoryContents
    {
        IFileInfo[] files;
        public DirectoryContents(IEnumerable<StaticFileIndex> files)
        {
            this.files = files.Select(o => new FileInfo(o)).ToArray();
        }
        public bool Exists => true;
        public IEnumerator<IFileInfo> GetEnumerator()
        {
            foreach (var f in files) yield return f;
        }
        IEnumerator IEnumerable.GetEnumerator() => files.GetEnumerator();
    }
    public class FileInfo : IFileInfo
    {
        StaticFileData file = null!;
        public FileInfo(StaticFileData file)
        {
            this.file = file;
            LastModified = file.UpdateTime;
            Name = Path.GetFileName(file.Path);
            Length = file.Content.Length;

        }
        public FileInfo(StaticFileIndex index)
        {
            Name = Path.GetFileName(index.Path);
            Length = index.Size;
            LastModified = index.LastUpdate;
        }
        public bool Exists => true;
        public bool IsDirectory => false;
        public DateTimeOffset LastModified { get; private set; }
        public long Length { get; private set; }
        public string Name { get; private set; }
        public string PhysicalPath => null!;
        public Stream CreateReadStream() => file == null ? null! : new MemoryStream(file.Content);
    }
}