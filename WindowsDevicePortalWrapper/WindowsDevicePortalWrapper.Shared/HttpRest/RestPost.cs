﻿//----------------------------------------------------------------------------------------------
// <copyright file="RestPost.cs" company="Microsoft Corporation">
//     Licensed under the MIT License. See LICENSE.TXT in the project root license information.
// </copyright>
//----------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace Microsoft.Tools.WindowsDevicePortal
{
    /// <content>
    /// HTTP POST Wrapper
    /// </content>
    public partial class DevicePortal
    {
        /// <summary>
        /// Calls the specified API with the provided body. This signature leaves
        /// off the optional response so callers who don't need a response body
        /// don't need to specify a type for it.
        /// </summary>
        /// <param name="apiPath">The relative portion of the uri path that specifies the API to call.</param>
        /// <param name="payload">The query string portion of the uri path that provides the parameterized data.</param>
        /// <returns>Task tracking the POST completion.</returns>
        private async Task Post(
            string apiPath,
            string payload = null)
        {
            await this.Post<NullResponse>(apiPath, payload);
        }

        /// <summary>
        /// Calls the specified API with the provided payload.
        /// </summary>
        /// <typeparam name="T">The type of the data for the HTTP response body (if present).</typeparam>
        /// <param name="apiPath">The relative portion of the uri path that specifies the API to call.</param>
        /// <param name="payload">The query string portion of the uri path that provides the parameterized data.</param>
        /// <returns>Task tracking the POST completion.</returns>
        private async Task<T> Post<T>(
            string apiPath,
            string payload = null) where T : new()
        {
            T data = default(T);

            Uri uri = Utilities.BuildEndpoint(
                this.deviceConnection.Connection,
                apiPath, 
                payload);

            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(T));

            using (Stream dataStream = await this.Post(uri))
            {
                if (dataStream != null)
                {
                    JsonFormatCheck<T>(dataStream);

                    object response = deserializer.ReadObject(dataStream);
                    data = (T)response;
                }
            }

            return data;
        }
    }
}
