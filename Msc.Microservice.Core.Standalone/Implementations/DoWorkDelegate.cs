// -------------------------------------------------------------------------------------------------------------------
// <copyright file="DoWorkDelegate.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Делегат выполнения асинхронной работы службы.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Threading;

namespace Msc.Microservice.Core.Standalone.Implementations
{
    /// <summary>
    /// Делегат выполнения асинхронной работы службы.
    /// </summary>
    /// <param name="serviceProvider">Провайдер микросервисов.</param>
    /// <param name="stopServiceToken">Токен останова службы.</param>
    public delegate void DoWorkDelegate(IServiceProvider serviceProvider, CancellationToken stopServiceToken);
}
