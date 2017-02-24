namespace CitroidForSlack.Plugins.CitDealer
{
	public interface IPerson
	{
		int Money { get; set; }
		string Name { get; }
		Gender Gender { get; }
		Personality Personality { get; }
	}

	public enum Gender
	{
		Male,
		Female
	}

	public enum Personality
	{
		/// <summary>
		/// 大人。
		/// </summary>
		Adult,
		/// <summary>
		/// 若者。
		/// </summary>
		Youngster,
		/// <summary>
		/// おこちゃま。
		/// </summary>
		Child,
		/// <summary>
		/// オタク。
		/// </summary>
		Nerd
	}

}