using System.Linq;
using System.Windows;

namespace DummyImageViewer
{
    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    public partial class App
    {
        /// <summary>
        /// Genera l'evento <see cref="E:System.Windows.Application.Startup" />.
        /// </summary>
        /// <param name="e">Oggetto <see cref="T:System.Windows.StartupEventArgs" /> che contiene i dati dell'evento.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args.Any())
            {
                Properties["Arg0"] = e.Args[0];
            }

            base.OnStartup(e);
        }
    }
}
