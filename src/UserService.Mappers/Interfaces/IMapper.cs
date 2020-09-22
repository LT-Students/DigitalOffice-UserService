namespace LT.DigitalOffice.UserService.Mappers.Interfaces
{
    /// <summary>
    /// Represents interface for a mapper in mapper pattern.
    /// Provides methods for converting an object of <see cref="TIn"/> type into an object of <see cref="TOut"/> type according to some rule.
    /// </summary>
    /// <typeparam name="TIn">Incoming object type.</typeparam>
    /// <typeparam name="TOut">Outgoing object type.</typeparam>
    public interface IMapper<in TIn, out TOut>
    {
        /// <summary>
        /// Convert an object of <see cref="TIn"/> type into an object of <see cref="TOut"/> type according to some rule.
        /// </summary>
        /// <param name="value">Specified object of <see cref="TIn"/> type.</param>
        /// <returns>The conversion result of <see cref="TOut"/> type.</returns>
        TOut Map(TIn value);
    }

    /// <summary>
    /// Represents mapper. Provides methods for converting objects of <see cref="TIn1"/> and <see cref="TIn2"/> types into an object of <see cref="TOut"/> type according to some rule.
    /// </summary>
    /// <typeparam name="TIn1">First incoming object type.</typeparam>
    /// <typeparam name="TIn2">Second incoming object type.</typeparam>
    /// <typeparam name="TOut">Outgoing object type.</typeparam>
    public interface IMapper<in TIn1, in TIn2, out TOut>
    {
        /// <summary>
        /// Convert objects of <see cref="TIn1"/> and <see cref="TIn2"/> types into an object of <see cref="TOut"/> type according to some rule.
        /// </summary>
        /// <param name="value1">Specified object of <see cref="TIn1"/> type.</param>
        /// <param name="value2">Specified object of <see cref="TIn2"/> type.</param>
        /// <returns>The conversion result of <see cref="TOut"/> type.</returns>
        TOut Map(TIn1 value1, TIn2 value2);
    }
}