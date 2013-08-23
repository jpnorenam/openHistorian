﻿//******************************************************************************************************
//  NullTreeScanner256.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  12/1/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using openHistorian.Collections.Generic;

namespace openHistorian.Collections
{
    /// <summary>
    /// Represents an empty tree scanner. 
    /// </summary>
    /// <remarks>
    /// This can be useful to return instead of null at times. Seeks will not throw exceptions and 
    /// scans will yield no results.
    /// To use this class. Call the static property <see cref="Instance"/>.
    /// </remarks>
    public class NullTreeScanner<TKey, TValue>
        : TreeScannerCoreBase<TKey, TValue>
        where TKey : class, new()
        where TValue : class, new()
    {
        /// <summary>
        /// Returns a static instance of this class
        /// </summary>
        public static TreeScannerCoreBase<TKey, TValue> Instance
        {
            get;
            private set;
        }

        static NullTreeScanner()
        {
            Instance = new NullTreeScanner<TKey, TValue>();
        }

        public override bool Read()
        {
            IsValid = false;
            return false;
        }

        public override unsafe void SeekToKey(TKey key)
        {
            IsValid = false;
        }
    }
}