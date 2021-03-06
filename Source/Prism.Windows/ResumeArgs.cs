﻿using System;
using Windows.ApplicationModel.Activation;

namespace Prism
{
    public class ResumeArgs : IResumeArgs, IActivatedEventArgs
    {
        public ActivationKind Kind { get; set; }
        public ApplicationExecutionState PreviousExecutionState { get; set; }
        public SplashScreen SplashScreen { get; set; }
        public DateTime? SuspendDate { get; set; }
    }
}
