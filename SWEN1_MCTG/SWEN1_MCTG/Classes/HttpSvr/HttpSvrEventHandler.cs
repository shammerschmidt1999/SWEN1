﻿using System;



namespace SWEN1_MCTG.Classes.HttpSvr
{
    /// <summary>This delegate is used for <see cref="HttpSvr"/> events.</summary>
    /// <param name="sender">Sending object.</param>
    /// <param name="e">Event arguments.</param>
    public delegate void HttpSvrEventHandler(object sender, HttpSvrEventArgs e);
}
