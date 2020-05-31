// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE;

namespace SE.Storage.Http
{
    public partial class HttpRemoteResource
    {
        public static class Methods
        {
            /// <summary>
            /// The HEAD method asks for a response identical to that of a GET request, 
            /// but without the response body
            /// </summary>
            public const string Head = "HEAD";

            /// <summary>
            /// The POST method is used to submit an entity to the specified resource, 
            /// often causing a change in state or side effects on the server
            /// </summary>
            public const string Post = "POST";

            /// <summary>
            /// The GET method requests a representation of the specified resource. 
            /// Requests using GET should only retrieve data
            /// </summary>
            public const string Get = "GET";

            /// <summary>
            /// The PUT method replaces all current representations of the target 
            /// resource with the request payload
            /// </summary>
            public const string Put = "PUT";

            /// <summary>
            /// The PATCH method is used to apply partial modifications to a resource
            /// </summary>
            public const string Patch = "PATCH";

            /// <summary>
            /// The DELETE method deletes the specified resource
            /// </summary>
            public const string Delete = "DELETE";
        }
    }
}
