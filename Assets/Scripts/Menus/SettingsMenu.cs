using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
    {
        public Text textversion;

        public AudioMixer audioMixer;
        
        public TMP_Dropdown resolutionDropdown;

        public Slider volumeSlider;

        public Toggle fullscreenToggle;

        public TMP_Dropdown qualiteDropdown;
        
        private Resolution[] resolutions;

        private void Start()
        {
            Versioning();

            //Volume
            float value;

        
            //Plein écran
            fullscreenToggle.isOn = Screen.fullScreen;

            //Qualité
            var qualityLevel = QualitySettings.GetQualityLevel();
            qualiteDropdown.value = qualityLevel;


            //Résolution
            resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();

            var options = new List<string>();

            var currentResolution = 0;

            for (var i = 0; i < resolutions.Length; i++)
            {
                var option = resolutions[i].width + ":" + resolutions[i].height + " (" + resolutions[i].refreshRateRatio.ToString() + " Hz)";
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height) {
                    currentResolution = i;
                }
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolution;
            resolutionDropdown.RefreshShownValue();
        }

        public void SetResolution(int resolutionIndex)
        {
            var resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }

        public void SetQuality(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
        }

        public void SetFullscreen(bool isfullscreen)
        {
            Screen.fullScreen = isfullscreen;
        }

        public void Menu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        private void Versioning()
        {
            var version = Application.unityVersion;
            textversion.text = version;
        }
    }
