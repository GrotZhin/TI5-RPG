using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


public class TimelineCameraBinder : MonoBehaviour
{
    public PlayableDirector director;
    public string cinemachineTrackName;
 
    public void VincularCinemachineBrain(GameObject mainCameraGeral)
    {

        CinemachineBrain brain = mainCameraGeral.GetComponent<CinemachineBrain>();
        TimelineAsset timeline = director.playableAsset as TimelineAsset;
        foreach (var track in timeline.GetOutputTracks())
        {
            if (track.name == cinemachineTrackName)
            {
                director.SetGenericBinding(track, brain);
                break;
            }
        }
    }
}