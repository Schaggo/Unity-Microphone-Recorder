using UnityEngine;  
using System.Collections;  
using UnityEngine.UI;
using System;
using UnityEditor.Recorder;
using System.Windows.Input;
using UnityEditor;


/// <summary>
/// This script triggers audio recording via savwav by pressing F10 to synchronize with UnityEditor.Recorder
/// Unity Recorder and RecordAudio functions are called the same...
/// </summary>
  
[RequireComponent (typeof (AudioSource))]  
public class RecordAudio : MonoBehaviour
{  
    public bool directMonitoring = false;
    AudioClip clip;
    public string filePath;
    public string fileName;	
    public Dropdown m_dropdown;
    private AudioSource goAudioSource;  
    public int preferedSampleRate;
    int maxFreq;
    int minFreq;
    public bool isRecording;
 
  
  
    void Start()   
    { 
        if(Microphone.devices.Length <= 0)  
        {  
            //Throw a warning message at the console if there isn't  
            Debug.LogWarning("Microphone not connected!");  
        }  
       
        else 
        {  
            m_dropdown.ClearOptions();
            // populate dropdown with list of available devices
            foreach (var device in Microphone.devices)
            {       
            m_dropdown.options.Add (new Dropdown.OptionData() {text=device.ToString()});   
            }

            //Get the selected microphone recording capabilities  
            Microphone.GetDeviceCaps(Microphone.devices[m_dropdown.value], out minFreq, out maxFreq);  
  
            // if max min = 0 - device has no limitations 
            if(minFreq == 0 && maxFreq == 0)  
            {  
                maxFreq = preferedSampleRate;  
            }  
            //Get the attached AudioSource component  
            goAudioSource = this.GetComponent<AudioSource>();      
        }  
    }  

    void Update()
    {   
        if ( Input.GetKeyDown(KeyCode.Space) && isRecording == false)
        {
            StartRecording();
        }
     
        else if (Input.GetKeyDown(KeyCode.Space) &&  isRecording == true)
        {
            StopRecording();
        }       
    }
    
    public void StartRecording()
    { 
        goAudioSource.clip = Microphone.Start("", true, 300, maxFreq);  
        isRecording = true;
        
        if (directMonitoring)
        {   
            goAudioSource.Play();
        }
        // Start animation recording in Unity Recorder
        var recorderWindow = EditorWindow.GetWindow<RecorderWindow>();
        recorderWindow.StartRecording();
        
        // Calling editorWindow causes focus to switch to that window - revert focus to gameview
        if (Application.isFocused == false)
        {
        EditorApplication.ExecuteMenuItem("Window/General/Game");
        }

        Debug.Log("Recodring Audio on " +Microphone.devices[m_dropdown.value]);
    }

    public void StopRecording()
    {
        goAudioSource.clip = SavWav.TrimSilence(goAudioSource.clip, 0);
        isRecording = false;  
        SavWav.Save(fileName,filePath, goAudioSource.clip);    
        Microphone.End("");
        
        // Stop Unity Recorder
        var recorderWindow = EditorWindow.GetWindow<RecorderWindow>();
        recorderWindow.StopRecording();
        
        // Calling editorWindow causes focus to switch to that window - revert focus to gameview
        if (Application.isFocused == false)
        {
        EditorApplication.ExecuteMenuItem("Window/General/Game");
        }
        Debug.Log("Saved Anim");
    }

   private void OnDestroy() 
   {
       if (isRecording)
       StopRecording();    
   }
}  
