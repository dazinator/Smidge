﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using Smidge.CompositeFiles;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Smidge.FileProcessors;
using Smidge.Options;
using Xunit;

namespace Smidge.Tests
{
    public class SmidgeHelperTests
    {
        private readonly ISmidgeConfig _config = Mock.Of<ISmidgeConfig>();
        private readonly IUrlManager _urlManager = Mock.Of<IUrlManager>();
        private readonly IHostingEnvironment _hostingEnvironment = Mock.Of<IHostingEnvironment>();
        private readonly IFileProvider _fileProvider = Mock.Of<IFileProvider>();
        private readonly IHasher _hasher = Mock.Of<IHasher>();
        private readonly IEnumerable<IPreProcessor> _preProcessors = Mock.Of<IEnumerable<IPreProcessor>>();

        private readonly DynamicallyRegisteredWebFiles _dynamicallyRegisteredWebFiles;
        private readonly FileSystemHelper _fileSystemHelper;
        private readonly PreProcessManager _preProcessManager;
        private Bundles _bundles;
        private Mock<IOptions<Bundles>> _bundlesOptions;
        private Mock<IOptions<SmidgeOptions>> _smidgeOptions;
        private readonly PreProcessPipelineFactory _processorFactory;
        private readonly BundleManager _bundleManager;
        private readonly IRequestHelper _requestHelper;
        //private readonly Mock<IHttpContextAccessor> _httpContextAccessor;
        //private Mock<HttpContext> _httpContext;


        public SmidgeHelperTests()
        {
            //  var config = Mock.Of<ISmidgeConfig>();

            _dynamicallyRegisteredWebFiles = new DynamicallyRegisteredWebFiles();
            _fileSystemHelper = new FileSystemHelper(_hostingEnvironment, _config, _fileProvider);
            _preProcessManager = new PreProcessManager(_fileSystemHelper, _hasher);

            _bundles = new Bundles();
            _bundlesOptions = new Mock<IOptions<Bundles>>();
            _bundlesOptions.Setup(opt => opt.Value).Returns(_bundles);

            _smidgeOptions = new Mock<IOptions<SmidgeOptions>>();
            _smidgeOptions.Setup(opt => opt.Value).Returns(new SmidgeOptions());

            _processorFactory = new PreProcessPipelineFactory(_preProcessors, 
                new FileProcessingConventions(_smidgeOptions.Object, new List<IFileProcessingConvention>()));
            _bundleManager = new BundleManager(_fileSystemHelper, _bundlesOptions.Object, _processorFactory);

            _requestHelper = Mock.Of<IRequestHelper>();
        }


        [Fact]
        public async Task Generate_Css_Urls_For_Non_Existent_Bundle_Throws_Exception()
        {


            var sut = new SmidgeHelper(
                _dynamicallyRegisteredWebFiles, _preProcessManager, _fileSystemHelper, 
                _hasher, _bundleManager, _processorFactory, _urlManager, _requestHelper);

            var exception = await Assert.ThrowsAsync<BundleNotFoundException>
                    (
                        async () => await sut.GenerateCssUrlsAsync("DoesntExist", true)

                    );

        }


        [Fact]
        public async Task Generate_Js_Urls_For_Non_Existent_Bundle_Throws_Exception()
        {

            var sut = new SmidgeHelper(
                _dynamicallyRegisteredWebFiles, _preProcessManager, _fileSystemHelper, 
                _hasher, _bundleManager, _processorFactory, _urlManager, _requestHelper);

            var exception = await Assert.ThrowsAsync<BundleNotFoundException>
                    (
                        async () => await sut.GenerateJsUrlsAsync("DoesntExist", true)
                    );


        }

        [Fact]
        public async Task CssHere_HtmlString_For_Non_Existent_Css_Bundle_Throws_Exception()
        {

            var sut = new SmidgeHelper(
                _dynamicallyRegisteredWebFiles, _preProcessManager, _fileSystemHelper, 
                _hasher, _bundleManager, _processorFactory, _urlManager, _requestHelper);

            var exception = await Assert.ThrowsAsync<BundleNotFoundException>
                    (
                        async () =>
                        {
                            var result = await sut.CssHereAsync("doesn't exist");
                        }

                    );


        }


        [Fact]
        public async Task JsHere_HtmlString_For_Non_Existent_Css_Bundle_Throws_Exception()
        {

            var sut = new SmidgeHelper(
                _dynamicallyRegisteredWebFiles, _preProcessManager, _fileSystemHelper, 
                _hasher, _bundleManager, _processorFactory, _urlManager, _requestHelper);

            var exception = await Assert.ThrowsAsync<BundleNotFoundException>
                    (
                        async () =>
                        {
                            var result = await sut.JsHereAsync("doesn't exist");
                        }

                    );


        }



    }
}