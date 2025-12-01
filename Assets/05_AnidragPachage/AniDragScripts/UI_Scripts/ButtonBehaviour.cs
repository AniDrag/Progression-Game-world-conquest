using AniDrag.Utility;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;
public class ButtonBehaviour : MonoBehaviour
{
    public enum ButtonAction
    {
        SoundOnly,
        ToggleThis,
        ToggleChild,
        ToggleGrandChild,
        ToggleParent,
        ToggleGrandParent,
        ToggleTarget
    }
    public enum EnableDisable { Enable, Dissable }

    //Button Settings
    public bool useAudio = true;
    public ButtonAction action = ButtonAction.SoundOnly;
    public EnableDisable enableDisable = EnableDisable.Dissable;

    //Sound Settings 
    public AudioClip audioClip;
    public AudioMixerGroup audioOutput;
    [Range(0.1f, 3f)] public float pitch = 1f;
    [Range(0f, 1f)] public float volume = 1f;
    public bool randomPitch = false;

    //Target Details 
    [Tooltip("Used only when action = ToggleTarget")]
    public GameObject target;
    public int childIndex;
    public int grandchildIndex;

    //Events
    public UnityEvent onClick;

    private Button _button;
    private AudioSource _audio;

#if UNITY_EDITOR
    private void OnValidate()
    {
        // AUDIO SOURCE AUTO HANDLER
        _audio = GetComponent<AudioSource>();

        //if (!useAudio)
        //{
        //    if (_audio != null) DestroyImmediate(_audio);
        //    return;
        //}
        //else if (_audio == null) _audio = gameObject.AddComponent<AudioSource>();
        if(_audio != null ){
            _audio.playOnAwake = false;
            _audio.clip = audioClip;
        }
        if (audioOutput != null) _audio.outputAudioMixerGroup = audioOutput;

        // BUTTON CHECK
        if (_button == null)
            _button = GetComponent<Button>();
        if (_button == null)
            Debug.LogWarning($"{name}: No Button component found!");
    }
    [ContextMenu("Clean Component")]
    public void CleanComponent()
    {
        if(!useAudio)
        {
            if (_audio != null) DestroyImmediate(_audio);
            return;
        }
        else if (_audio == null) _audio = gameObject.AddComponent<AudioSource>();
    }

#endif

    #region Unity Functions
    private void OnEnable()
    {
        if (_button == null) _button = GetComponent<Button>();
        _button.onClick.AddListener(Fire);
    }

    private void OnDisable()
    {
        if (_button != null)
            _button.onClick.RemoveListener(Fire);
    }

    #endregion

    #region Button Functions
    private void Fire()
    {
        PlaySound();
        onClick?.Invoke();
        ExecuteAction();
    }

    private void PlaySound()
    {
        if (!useAudio || _audio == null || audioClip == null) return;

        _audio.pitch = randomPitch ? Random.Range(0.9f, 1.1f) : pitch;

        _audio.Stop();
        _audio.PlayOneShot(audioClip, volume);
    }

    private void ExecuteAction()
    {
        GameObject resolv = null;

        switch (action)
        {
            case ButtonAction.SoundOnly:
                return;

            case ButtonAction.ToggleThis:
                resolv = gameObject;
                break;

            case ButtonAction.ToggleTarget:
                resolv = target;
                break;

            case ButtonAction.ToggleChild:
                if (childIndex >= transform.childCount) { Debug.LogError($"Child index too high on {name}"); return; }
                resolv = transform.GetChild(childIndex).gameObject;
                break;

            case ButtonAction.ToggleGrandChild:
                if (childIndex >= transform.childCount) { Debug.LogError($"Child index too high on {name}"); return; }
                if (grandchildIndex >= transform.GetChild(childIndex).childCount) { Debug.LogError($"Grandchild index too high on {name}"); return; }
                resolv = transform.GetChild(childIndex).GetChild(grandchildIndex).gameObject;
                break;

            case ButtonAction.ToggleParent:
                if (transform.parent == null) return;
                resolv = transform.parent.gameObject;
                break;

            case ButtonAction.ToggleGrandParent:
                if (transform.parent == null || transform.parent.parent == null) return;
                resolv = transform.parent.parent.gameObject;
                break;
        }

        if (enableDisable == EnableDisable.Dissable)
            resolv.SetActive(false);
        else
            resolv.SetActive(true);
    }
    #endregion
}