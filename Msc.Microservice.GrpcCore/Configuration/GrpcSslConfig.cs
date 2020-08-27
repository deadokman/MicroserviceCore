// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GrpcSslConfig.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Конфигурация для Grpc-сервера. Настройки SSL.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Msc.Microservice.GrpcCore.Configuration
{
    /// <summary>
    /// Настройка SSL.
    /// </summary>
    public class GrpcSslConfig
    {
        /// <summary>
        /// Родительский сертификат.
        /// </summary>
        public string CaCert { get; set; }

        /// <summary>
        /// PEM encoded certificate chain.
        /// </summary>
        /// <remarks>
        /// Иерархия сертификатов — это структура, которая позволяет людям проверять валидность сертификата эмитента.
        /// Сертификаты подписываются закрытыми ключами тех сертификатов, которые находятся выше в иерархии сертификатов.
        /// Поэтому достоверность данного сертификата определяется достоверностью сертификата, которым он был подписан.
        /// Наивысший сертификат в цепочке называется корневым (Root certificate).
        /// </remarks>
        public string CertificateChain { get; set; }

        /// <summary>
        /// PEM encoded private key.
        /// </summary>
        /// <remarks>
        /// PEM – наиболее популярный формат среди сертификационных центров.
        /// PEM сертификаты могут иметь расширение .pem, .crt, .cer, и .key (файл приватного ключа).
        /// Она представляют собой ASCII файлы, закодированные по схеме Base64.
        /// </remarks>
        public string PrivateKey { get; set; }

        /// <summary>
        /// Путь к сертификатам.
        /// </summary>
        public string Path { get; set; }
    }
}