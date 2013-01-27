﻿//******************************************************************************************************
//  WeakAction.cs - Gbtc
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
//  1/16/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GSF.Threading
{
    /// <summary>
    /// Provides a weak referenced action delegate. 
    /// </summary>
    public class WeakAction : WeakDelegateBase<Action>
    {
        public WeakAction(Action target)
            : base(target)
        {
        }
        public bool TryInvoke()
        {
            return TryInvokeInternal(null);
        }
    }

    /// <summary>
    /// Provides a weak referenced action delegate. 
    /// </summary>
    public class WeakAction<T> : WeakDelegateBase<Action<T>>
    {
        public WeakAction(Action<T> target)
            : base(target)
        {
        }
        public bool TryInvoke(T obj)
        {
            return TryInvokeInternal(new object[] {obj});
        }
        
    }

    /// <summary>
    /// Provides a weak referenced action delegate. 
    /// </summary>
    public class WeakEventHandler<T> : WeakDelegateBase<EventHandler<T>>
        where T : EventArgs
    {
        public WeakEventHandler(EventHandler<T> target)
            : base(target)
        {
        }

        public bool TryInvoke(object sender, T e)
        {
            return TryInvokeInternal(new object[] { sender, e });
        }
    }
}
