﻿/**
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

using Newtonsoft.Json;
using System.Collections.Generic;

namespace IBM.VCA.Watson.Watson.Model
{
    /// <summary>
    /// Terms from the request that are identified as entities.
    /// </summary>
    public class EntityResponse
    {
        /// <summary>
        /// The recognized entity from a term in the input.
        /// </summary>
        [JsonProperty("entity")]
        public string Entity { get; set; }

        /// <summary>
        /// Zero-based character offsets that indicate where the entity value begins and ends in the input text.
        /// </summary>
        [JsonProperty("location")]
        public List<int> Location { get; set; }

        /// <summary>
        /// The term in the input text that was recognized
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("confidence")]
        public int Confidence { get; set; }

    }
}
