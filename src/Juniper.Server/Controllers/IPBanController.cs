using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Juniper.HTTP.Server.Controllers
{
    public sealed class IPBanController : AbstractRequestHandler
    {
        private readonly FileInfo banFile;

        public List<CIDRBlock> Blocks { get; } = new List<CIDRBlock>();

        public event EventHandler<EventArgs<CIDRBlock>> BanAdded;
        public event EventHandler<EventArgs<CIDRBlock>> BanRemoved;

        public IPBanController()
        { }

        public IPBanController(IEnumerable<CIDRBlock> blocks)
            : base(null, int.MinValue, HttpProtocols.All, HttpMethods.All)
        {
            Blocks.AddRange(blocks);
            Blocks.Sort();
        }

        public IPBanController(Stream banFileStream)
            : this(CIDRBlock.Load(banFileStream))
        { }

        public IPBanController(FileInfo banFile)
            : this(CIDRBlock.Load(banFile))
        {
            this.banFile = banFile;
        }

        public IPBanController(string banFileName)
            : this(CIDRBlock.Load(banFileName))
        {
            banFile = new FileInfo(banFileName);
        }

        private CIDRBlock GetMatchingBlock(IPAddress address)
        {
            return Blocks.Find(block => block.Contains(address));
        }

        public override bool IsMatch(HttpListenerRequest request)
        {
            var block = GetMatchingBlock(request.RemoteEndPoint.Address);
            if (block is object)
            {
                return true;
            }

            if (request.Url.PathAndQuery.Contains(".php"))
            {
                var added = new List<CIDRBlock>();
                var removed = new List<CIDRBlock>();

                block = new CIDRBlock(request.RemoteEndPoint.Address);
                OnWarning($"Auto-banning {block}");
                added.Add(block);
                Blocks.Add(block);
                Blocks.Sort();

                for (int i = Blocks.Count - 1; i > 0; --i)
                {
                    var right = Blocks[i];
                    var left = Blocks[i - 1];
                    if (left.Overlaps(right))
                    {
                        Blocks[i - 1] = left + right;
                        Blocks.RemoveAt(i);

                        added.Remove(left);
                        removed.MaybeAdd(left);

                        added.Remove(right);
                        removed.MaybeAdd(right);

                        removed.Remove(Blocks[i - 1]);
                        added.MaybeAdd(Blocks[i - 1]);
                    }
                }

                foreach(var b in removed)
                {
                    OnBanRemoved(b);
                }

                foreach(var b in added)
                {
                    OnBanAdded(b);
                }

                if (banFile is object)
                {
                    CIDRBlock.Save(Blocks, banFile);
                }

                return true;
            }

            return false;
        }

        public override bool CanContinue(HttpListenerRequest request)
        {
            var block = GetMatchingBlock(request.RemoteEndPoint.Address);
            return base.CanContinue(request)
                && block is null;
        }

        public override Task InvokeAsync(HttpListenerContext context)
        {
            var block = GetMatchingBlock(context.Request.RemoteEndPoint.Address);
            OnInfo($"{context.Request.RemoteEndPoint} is banned by {block}.");
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return Task.CompletedTask;
        }

        private void OnBanAdded(CIDRBlock block)
        {
            BanAdded?.Invoke(this, new EventArgs<CIDRBlock>(block));
        }

        private void OnBanRemoved(CIDRBlock block)
        {
            BanRemoved?.Invoke(this, new EventArgs<CIDRBlock>(block));
        }
    }
}
