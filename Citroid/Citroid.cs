using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Citroid
{
	public interface IBot
	{
		void OnEnabled(Citroid citroid);
		void OnDisabled(Citroid citroid);
	}

	/// <summary>
	/// 一般的なメッセージに返信するBotのインターフェイスです。
	/// </summary>
	public interface IPostBot : IBot
	{
		
	}

	public interface IPost
	{
		IUser Sender { get; }
		string Text { get; }
	}

	public interface IUser
	{

	}



	/// <summary>
	/// 自分に対して送信されるメッセージに返信するBotのインターフェイスです。
	/// </summary>
	public interface IMentionBot : IBot
	{

	}

    public abstract class Citroid
    {

    }
}
