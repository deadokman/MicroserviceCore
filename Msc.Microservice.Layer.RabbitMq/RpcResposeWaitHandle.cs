// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RpcResposeWaitHandle.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Класс синхронизации с передачей объекта
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Threading;

namespace Msc.Microservice.Layer.RabbitMq
{
    /// <summary>
    /// Класс синхронизации с передачей объекта.
    /// </summary>
    public class RpcResposeWaitHandle
    {
        private ManualResetEventSlim _mres;

        /// <summary>
        /// Передаваемый в ожидающий поток объект.
        /// </summary>
        public object Payload { get; set; }

        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public RpcResposeWaitHandle()
        {
            _mres = new ManualResetEventSlim(false);
        }

        /// <summary>
        /// Ожидать результата обработки по RPC.
        /// </summary>
        /// <param name="span">ТАймаут.</param>
        public void WaitOne(TimeSpan span)
        {
            _mres.Wait(span);
        }

        /// <summary>
        /// Установить защелку в сигнальное положение.
        /// </summary>
        public void Set()
        {
            _mres.Set();
        }

        /// <summary>
        /// Сброс.
        /// </summary>
        public void Reset()
        {
            _mres.Reset();
        }

        /// <summary>
        /// Установлено сигнальное состояние.
        /// </summary>
        /// <returns>true если установлено сигнальное состояние.</returns>
        public bool IsSet => _mres.IsSet;
    }
}
