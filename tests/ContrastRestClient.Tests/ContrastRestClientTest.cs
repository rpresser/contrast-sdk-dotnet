﻿#region LICENSE
// Copyright (c) 2019, Contrast Security, Inc.
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
// 
// Redistributions of source code must retain the above copyright notice, this list of
// conditions and the following disclaimer.
// 
// Redistributions in binary form must reproduce the above copyright notice, this list of
// conditions and the following disclaimer in the documentation and/or other materials
// provided with the distribution.
// 
// Neither the name of the Contrast Security, Inc. nor the names of its contributors may
// be used to endorse or promote products derived from this software without specific
// prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY
// EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
// MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL
// THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT
// OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF
// THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Contrast;
using Contrast.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ContrastRestClient.Tests
{
    [TestClass]
    public class ContrastRestClientTest
    {
        private Contrast.Http.ContrastRestClient CreateClientThatReturnStatusCode(System.Net.HttpStatusCode statusCode)
        {
            var mockClient = new Mock<IHttpClient>();
            mockClient.Setup(c => c.GetAsync(It.IsAny<String>())).Returns(
                Task.FromResult<HttpResponseMessage>(new HttpResponseMessage(statusCode)
                {
                    Content = new StreamContent( new MemoryStream() )
                })
                );

            var client = new Contrast.Http.ContrastRestClient(mockClient.Object);
            return client;
        }

        [TestMethod]
        public void GetResponseStream_OkResponse_NoException()
        {
            var client = CreateClientThatReturnStatusCode(System.Net.HttpStatusCode.OK);

            client.GetResponseStream("arbitrary");
        }

        [TestMethod, ExpectedException(typeof(ContrastApiException))]
        public void GetResponseStream_UnauthorizedResponse_ContrastApiExceptionThrown()
        {
            var client = CreateClientThatReturnStatusCode(System.Net.HttpStatusCode.Unauthorized);

            client.GetResponseStream("arbitrary");
        }

        [TestMethod, ExpectedException(typeof(ContrastApiException))]
        public void GetResponseStream_RedirectResponse_ContrastApiExceptionThrown()
        {
            var client = CreateClientThatReturnStatusCode(System.Net.HttpStatusCode.Redirect);

            client.GetResponseStream("arbitrary");
        }

        [TestMethod, ExpectedException(typeof(ResourceNotFoundException))]
        public void GetResponseStream_NotFoundResponse_ResourceNotFoundExceptionThrown()
        {
            var client = CreateClientThatReturnStatusCode(System.Net.HttpStatusCode.NotFound);

            client.GetResponseStream("arbitrary");
        }
    }
}
