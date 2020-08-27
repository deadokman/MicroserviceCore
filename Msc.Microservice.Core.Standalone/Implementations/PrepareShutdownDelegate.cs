// -------// -------------------------------------------------------------------------------------------------------------------
// <copyright file="PrepareShutdownDelegate.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Делегат для реакции на выклчение службы
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Msc.Microservice.Core.Standalone.Implementations
{
    /// <summary>
    /// Делегат для метода выключения слудбы.
    /// </summary>
    /// <param name="sp">Service provider.</param>
    public delegate void PrepareShutdownDelegate(IServiceProvider sp);
}
