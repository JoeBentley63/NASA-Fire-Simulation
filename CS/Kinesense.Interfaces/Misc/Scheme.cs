namespace Kinesense.Interfaces
{
	public struct Scheme
	{
		private readonly string _code;
		private readonly string _name;

		public string Code { get { return _code; } }
		public string Name { get { return _name; } }

		public Scheme(string code)
		{
			_code = code;
			_name = "";
		}

		public Scheme(string code, string name)
		{
			_code = code;
			_name = name;
		}

		public override string ToString()
		{
			return _name;
		}
	}
}
