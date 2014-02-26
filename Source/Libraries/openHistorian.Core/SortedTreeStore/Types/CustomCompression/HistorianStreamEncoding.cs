﻿//******************************************************************************************************
//  HistorianCompressedStream.cs - Gbtc
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
//  8/10/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.IO;
using GSF.IO;
using GSF.SortedTreeStore.Encoding;
using openHistorian.Collections;
using GSF.SortedTreeStore.Net.Initialization;

namespace GSF.SortedTreeStore.Net.Compression
{

    public class CreateHistorianStreamEncoding
         : CreateCombinedValuesBase
    {
        // {0418B3A7-F631-47AF-BBFA-8B9BC0378328}
        public readonly static Guid TypeGuid = new Guid(0x0418b3a7, 0xf631, 0x47af, 0xbb, 0xfa, 0x8b, 0x9b, 0xc0, 0x37, 0x83, 0x28);

        public override Type KeyTypeIfNotGeneric
        {
            get
            {
                return typeof(HistorianKey);
            }
        }

        public override Type ValueTypeIfNotGeneric
        {
            get
            {
                return typeof(HistorianValue);
            }
        }

        public override Guid Method
        {
            get
            {
                return TypeGuid;
            }
        }

        public override DoubleValueEncodingBase<TKey, TValue> Create<TKey, TValue>()
        {
            return (DoubleValueEncodingBase<TKey, TValue>)(object)(new HistorianStreamEncoding());
        }
    }

    public class HistorianStreamEncoding
        : DoubleValueEncodingBase<HistorianKey, HistorianValue>
    {
        

        public override bool UsesPreviousKey
        {
            get
            {
                return true;
            }
        }

        public override bool UsesPreviousValue
        {
            get
            {
                return false;
            }
        }

        public override int MaxCompressionSize
        {
            get
            {
                return 55; //3 extra bytes just to be safe.
            }
        }

        public override bool ContainsEndOfStreamSymbol
        {
            get
            {
                return true;
            }
        }

        public override byte EndOfStreamSymbol
        {
            get
            {
                return 255;
            }
        }

        public unsafe override void Encode(BinaryStreamBase stream, HistorianKey prevKey, HistorianValue prevValue, HistorianKey currentKey, HistorianValue currentValue)
        {
            if (currentKey.Timestamp == prevKey.Timestamp
                && ((currentKey.PointID ^ prevKey.PointID) < 64)
                && currentKey.EntryNumber == 0
                && currentValue.Value1 <= uint.MaxValue //must be a 32-bit value
                && currentValue.Value2 == 0
                && currentValue.Value3 == 0)
            {
                if (currentValue.Value1 == 0)
                {
                    stream.Write((byte)((currentKey.PointID ^ prevKey.PointID)));
                }
                else
                {
                    stream.Write((byte)((currentKey.PointID ^ prevKey.PointID) | 64));
                    stream.Write((uint)currentValue.Value1);
                }
                return;
            }

            byte code = 128;

            if (currentKey.Timestamp != prevKey.Timestamp)
                code |= 64;

            if (currentKey.EntryNumber != 0)
                code |= 32;

            if (currentValue.Value1 > uint.MaxValue)
                code |= 16;
            else if (currentValue.Value1 > 0)
                code |= 8;

            if (currentValue.Value2 != 0)
                code |= 4;

            if (currentValue.Value3 > uint.MaxValue)
                code |= 2;
            else if (currentValue.Value3 > 0)
                code |= 1;

            stream.Write(code);

            if (currentKey.Timestamp != prevKey.Timestamp)
                stream.Write7Bit(currentKey.Timestamp ^ prevKey.Timestamp);

            stream.Write7Bit(currentKey.PointID ^ prevKey.PointID);

            if (currentKey.EntryNumber != 0)
                stream.Write7Bit(currentKey.EntryNumber);

            if (currentValue.Value1 > uint.MaxValue)
                stream.Write(currentValue.Value1);
            else if (currentValue.Value1 > 0)
                stream.Write((uint)currentValue.Value1);

            if (currentValue.Value2 != 0)
                stream.Write(currentValue.Value2);

            if (currentValue.Value3 > uint.MaxValue)
                stream.Write(currentValue.Value3);
            else if (currentValue.Value3 > 0)
                stream.Write((uint)currentValue.Value3);

        }

        public override void Decode(BinaryStreamBase stream, HistorianKey prevKey, HistorianValue prevValue, HistorianKey key, HistorianValue value, out bool endOfStream)
        {
            endOfStream = false;
            byte code = stream.ReadUInt8();
            if (code == 255)
            {
                endOfStream = true;
                return;
            }

            if (code < 128)
            {
                if (code < 64)
                {
                    key.Timestamp = prevKey.Timestamp;
                    key.PointID = prevKey.PointID ^ code;
                    key.EntryNumber = 0;
                    value.Value1 = 0;
                    value.Value2 = 0;
                    value.Value3 = 0;
                }
                else
                {
                    key.Timestamp = prevKey.Timestamp;
                    key.PointID = prevKey.PointID ^ code ^ 64;
                    key.EntryNumber = 0;
                    value.Value1 = stream.ReadUInt32();
                    value.Value2 = 0;
                    value.Value3 = 0;
                }
             
                return ;
            }

            if ((code & 64) != 0) //T is set
                key.Timestamp = prevKey.Timestamp ^ stream.Read7BitUInt64();
            else
                key.Timestamp = prevKey.Timestamp;

            key.PointID = prevKey.PointID ^ stream.Read7BitUInt64();

            if ((code & 32) != 0) //E is set)
                key.EntryNumber = stream.Read7BitUInt64();
            else
                key.EntryNumber = 0;

            if ((code & 16) != 0) //V1 High is set)
                value.Value1 = stream.ReadUInt64();
            else if ((code & 8) != 0) //V1 low is set)
                value.Value1 = stream.ReadUInt32();
            else
                value.Value1 = 0;

            if ((code & 4) != 0) //V2 is set)
                value.Value2 = stream.ReadUInt64();
            else
                value.Value2 = 0;

            if ((code & 2) != 0) //V1 High is set)
                value.Value3 = stream.ReadUInt64();
            else if ((code & 1) != 0) //V1 low is set)
                value.Value3 = stream.ReadUInt32();
            else
                value.Value3 = 0;
           
            return ;
        }

        public override DoubleValueEncodingBase<HistorianKey, HistorianValue> Clone()
        {
            return new HistorianStreamEncoding();
        }
    }
}
