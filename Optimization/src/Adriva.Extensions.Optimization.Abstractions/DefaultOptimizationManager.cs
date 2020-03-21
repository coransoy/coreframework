using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Adriva.Extensions.Optimization.Abstractions
{
    internal sealed class DefaultOptimizationManager<TOptions> : IOptimizationManager where TOptions : OptimizationOptions, new()
    {
        private readonly IServiceProvider ServiceProvider;
        private readonly ILogger Logger;
        private readonly TOptions Options;

        private readonly Dictionary<string, List<ITransform>> TransformChains = new Dictionary<string, List<ITransform>>();
        private readonly List<IAssetLoader> Loaders = new List<IAssetLoader>();
        private readonly List<object> Formatters = new List<object>();
        private readonly IOptimizationEvents<TOptions> Events;
        private IAssetOrderer Orderer;


        public DefaultOptimizationManager(IServiceProvider serviceProvider, IOptions<TOptions> optionsAccessor, ILogger<DefaultOptimizationManager<TOptions>> logger)
        {
            this.ServiceProvider = serviceProvider;
            this.Options = optionsAccessor.Value;
            this.Events = this.ServiceProvider.GetService<IOptimizationEvents<TOptions>>(); //optional
            this.Logger = logger;

            this.Initialize();
        }

        private void Initialize()
        {
            this.Options.Loaders.ForEach(t =>
            {
                this.Loaders.Add((IAssetLoader)ActivatorUtilities.CreateInstance(this.ServiceProvider, t));
            });

            foreach (var pair in this.Options.TransformChains)
            {
                List<ITransform> transforms = new List<ITransform>();
                this.TransformChains.Add(pair.Key, transforms);
                pair.Value.ForEach(t =>
                {
                    transforms.Add((ITransform)ActivatorUtilities.CreateInstance(this.ServiceProvider, t));
                });
            }

            foreach (var formatterType in this.Options.Formatters)
            {
                this.Formatters.Add(ActivatorUtilities.CreateInstance(this.ServiceProvider, formatterType));
            }

            if (null == this.Options.OrdererOptions)
            {
                this.Orderer = (IAssetOrderer)ActivatorUtilities.CreateInstance(this.ServiceProvider, this.Options.Orderer);
            }
            else
            {
                this.Orderer = (IAssetOrderer)ActivatorUtilities.CreateInstance(this.ServiceProvider, this.Options.Orderer, this.Options.OrdererOptions);
            }

            this.Events.ServiceInitialized?.Invoke(this.ServiceProvider, this.Options);
        }

        public async Task<OptimizationResult> OptimizeAsync(IOptimizationContext context, string extension)
        {
            if (null == extension) extension = string.Empty;
            string fileExtension = "." + extension;

            var orderedAssets = this.Orderer.Order(context.Assets.Where(a => 0 == string.Compare(Path.GetExtension(a.Location.ToString()), fileExtension, StringComparison.OrdinalIgnoreCase)));

            if (!orderedAssets.Any())
            {
                return new OptimizationResult(Enumerable.Empty<Asset>());
            }

            foreach (var asset in orderedAssets)
            {
                foreach (var loader in this.Loaders)
                {
                    if (loader.CanLoad(asset))
                    {
                        using (var stream = await loader.OpenReadStreamAsync(asset))
                        {
                            await asset.SetContentAsync(stream);
                        }
                        break;
                    }
                }
            }

            IEnumerable<Asset> assets = orderedAssets;

            foreach (var transformPair in this.TransformChains.Where(x => 0 == string.Compare(x.Key, fileExtension, StringComparison.OrdinalIgnoreCase)))
            {
                foreach (ITransform transform in transformPair.Value)
                {
                    if (null == this.Events?.TransformRunning || this.Events.TransformRunning.Invoke(extension, this.Options, transform))
                    {
                        assets = await transform.TransformAsync(assets.ToArray());
                    }
                }
                break;
            }

            return new OptimizationResult(assets);
        }

    }
}