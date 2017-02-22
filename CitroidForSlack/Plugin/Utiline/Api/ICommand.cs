using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitroidForSlack.Plugins.Utiline.Api
{
	public interface ICommand
	{
		/// <summary>
		/// コマンドの名前を取得します。
		/// </summary>
		string Name { get; }
		/// <summary>
		/// コマンドの一覧を列挙するときや、コマンドが正しく実行されない場合に表示される使用法を取得します。
		/// </summary>
		string Usage { get; }
		/// <summary>
		/// コマンドを実行します。
		/// </summary>
		string Process(string[] args);
	}
}
