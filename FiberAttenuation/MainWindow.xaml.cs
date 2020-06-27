using System;
using System.Collections;
using System.Windows;

using System.Windows.Input;

namespace FiberAttenuation {
    /// Interaction logic for MainWindow.xaml
    public partial class MainWindow : Window {
        ArrayList loss = new ArrayList();
        ArrayList wavelengthList = new ArrayList();
        ArrayList markers = new ArrayList();
        private string actualCommand = "";
        private double attenuationCoffe = 1.70; // Attenuation Coefficient.
        public MainWindow() {
            InitializeComponent();
        }

        /*
         * Use based on no mouse avaliable.
         */
        private void CommandTextBox_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                // Validate the input 
                actualCommand = commandTextBox.Text;
                bool valid = DecipherCommand();
                if (valid) {
                    Graph plots = new Graph(loss, wavelengthList, markers);
                    plots.Show();
                }
            }

        }

        private bool DecipherCommand() {
            loss.Clear();
            wavelengthList.Clear();
            markers.Clear();
            String commandText = actualCommand;
            if (!commandText.Contains(";")) {
                throw new Exception("Invalid Command");
            } else {
                try {
                    string[] commands = commandText.Split(';'); // split on the semi colon
                    foreach (var cmd in commands) {
                        string command = cmd.Trim().ToUpper(); // uppercase everything
                        if (command.StartsWith("W")) { // Wavelength, can be multiple We shall use a graph maybe thats what marker means?
                            if (command.Contains("TO") && command.Contains("S")) { //  must have TO and have a Step
                                int stepIndex = command.IndexOf("S");
                                string step = command.Substring(stepIndex);
                                double stepValue = double.Parse(step.Substring(1));

                                if (stepValue > 10 || stepValue < 0.1) {
                                    throw new Exception("Invalid Command: Step must be between 0.1nm and 10nm");
                                }
                                string subCommand = command.Replace(step, "").Replace(" ", "").Replace("W", "");
                                string[] toSplit = subCommand.Split(new string[] { "TO" }, StringSplitOptions.None); // string array overloading.
                                if (double.Parse(toSplit[0]) < 1200 || double.Parse(toSplit[1]) > 1600) {
                                    throw new Exception("Invalid Command: wavelength must be between 1200nm and 1600nm");
                                }
                                double[] wavelengths = Array.ConvertAll(toSplit, s => double.Parse(s));

                                for (double i = wavelengths[0]; i <= wavelengths[1]; i += double.Parse(step.Substring(1))) {
                                    wavelengthList.Add(i);
                                }

                            } else {
                                double wavelength = double.Parse(command.Substring(1));
                                wavelengthList.Add(wavelength);
                            }

                        }
                        if (command.Replace(" ", "").StartsWith("M")) {
                            Console.WriteLine(command);
                            double markerPoint = double.Parse(command.Substring(1));
                            markers.Add(markerPoint);
                        }

                    }
                    // for each item in the wavelengths list calculate the attenuation.
                    calculateAttenuation();
                } catch (Exception e) {
                    MessageBox.Show("Please enter a valid command!" + e);
                    Console.WriteLine(e);
                }
            }
            return true;
        }
        /**
         * When supplying a list of wavelengths we will use the attenuation coff to calcuate the fiber attenuation using Rayleighs scattering. 
         */
        private void calculateAttenuation() {
            for (int i = 0; i < wavelengthList.Count; i++) {
                var lambdaZero = (double) wavelengthList[0];
                if (i == 0) {
                    loss.Add(attenuationCoffe * Math.Pow((lambdaZero / lambdaZero), 4));

                } else {
                    loss.Add(attenuationCoffe * Math.Pow((lambdaZero / (double)wavelengthList[i]), 4));
                }
            }
        }
    }
}

