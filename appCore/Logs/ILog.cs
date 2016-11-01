/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 15-08-2016
 * Time: 21:47
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using appCore.Templates;

namespace appCore.Logs
{
    /// <summary>
    /// Description of ILog.
    /// </summary>
    interface ILog<out T> where T : Template
	{
	}
}