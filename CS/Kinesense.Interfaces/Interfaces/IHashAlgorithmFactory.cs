using System.Security.Cryptography;

namespace Kinesense.Interfaces
{
	public interface IHashAlgorithmFactory
	{
		HashAlgorithm GetHashAlgorithm(string hashAlgorithmName);
	}
}