using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Juniper.HTTP.Server.Controllers
{
    public sealed class BanHammer : AbstractResponse
    {
        private readonly FileInfo banFile;

        public List<CIDRBlock> Blocks { get; } = new List<CIDRBlock>();

        public event EventHandler<EventArgs<CIDRBlock>> BanAdded;
        public event EventHandler<EventArgs<CIDRBlock>> BanRemoved;

        public BanHammer(IEnumerable<CIDRBlock> blocks = null)
            : base(int.MinValue, HttpProtocols.All, HttpMethods.All, 0, AnyAuth, MediaType.All)
        {
            if (blocks != null)
            {
                Blocks.AddRange(blocks);
                Blocks.Sort();
            }
        }

        public BanHammer(Stream banFileStream)
            : this(CIDRBlock.Load(banFileStream))
        { }

        public BanHammer(FileInfo banFile)
            : this(CIDRBlock.Load(banFile))
        {
            this.banFile = banFile;
        }

        public BanHammer(string banFileName)
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

                for (var i = Blocks.Count - 1; i > 0; --i)
                {
                    var right = Blocks[i];
                    var left = Blocks[i - 1];
                    if (left.Overlaps(right))
                    {
                        Blocks[i - 1] = left + right;
                        Blocks.RemoveAt(i);

                        _ = added.Remove(left);
                        _ = removed.MaybeAdd(left);

                        _ = added.Remove(right);
                        _ = removed.MaybeAdd(right);

                        _ = removed.Remove(Blocks[i - 1]);
                        _ = added.MaybeAdd(Blocks[i - 1]);
                    }
                }

                foreach (var b in removed)
                {
                    OnBanRemoved(b);
                }

                foreach (var b in added)
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

        public override Task InvokeAsync(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;
            var block = GetMatchingBlock(request.RemoteEndPoint.Address);
            OnInfo($"{request.RemoteEndPoint} is banned by {block}.");
            response.SetStatus(HttpStatusCode.Unauthorized);
            return response.SendTextAsync("Unauthorized");
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
