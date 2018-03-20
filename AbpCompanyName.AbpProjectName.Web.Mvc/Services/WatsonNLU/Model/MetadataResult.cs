/**
* Copyright 2017 IBM Corp. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

using System.Collections.Generic;
using Newtonsoft.Json;

namespace IBM.WatsonDeveloperCloud.NaturalLanguageUnderstanding.v1.Model
{
    /// <summary>
    /// The Authors, Publication Date, and Title of the document. Supports URL and HTML input types.
    /// </summary>
    public class MetadataResult
    {
        /// <summary>
        /// The authors of the document.
        /// </summary>
        /// <value>The authors of the document.</value>
        [JsonProperty("authors", NullValueHandling = NullValueHandling.Ignore)]
        public List<Author> Authors { get; set; }
        /// <summary>
        /// The publication date in the format ISO 8601.
        /// </summary>
        /// <value>The publication date in the format ISO 8601.</value>
        [JsonProperty("publication_date", NullValueHandling = NullValueHandling.Ignore)]
        public string PublicationDate { get; set; }
        /// <summary>
        /// The title of the document.
        /// </summary>
        /// <value>The title of the document.</value>
        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }
    }

}
