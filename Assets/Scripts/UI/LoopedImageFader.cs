using System.Collections;
using System.Collections.Generic;
using EyegazeCore;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class LoopedImageFader : MonoBehaviour
{
    [SerializeField]
    private GameObject _imageContainer;

    [SerializeField]
    private StandardAccessibleButton _nextButton;

    [SerializeField]
    private StandardAccessibleButton _prevButton;

    [SerializeField]
    private Animator _animator;

    private int _index = 0;

    private List<GameObject> _slides = new List<GameObject>();
    private List<Image> _images = new List<Image>();
    private List<Image> _locks = new List<Image>();

    public Signal<int> ON_SELECT_UPDATE = new Signal<int>();

    public void Init(List<Sprite> sprites)
    {
        _index = 0;

        //clear
        foreach (Transform child in _imageContainer.transform)
        {
            _slides.Clear();
            _images.Clear();
            _locks.Clear();
            GameObject.Destroy(child.gameObject);
        }

        //add content (all locked state by default)
        if (sprites.Count > 0)
        {
            foreach(Sprite s in sprites)
            {
                GameObject go = new GameObject();
                go.transform.localPosition = Vector3.zero;
                go.transform.SetParent(_imageContainer.transform, false);
                Image i = go.AddComponent<Image>();
                i.enabled = true;
                _images.Add(i);
                i.sprite = s;
                Color c = Color.white;
                c.a = 0.3f;
                i.color = c;
                i.SetNativeSize();
                _slides.Add(go);
                

                GameObject lGo = new GameObject();
                lGo.transform.localPosition = Vector3.zero;
                lGo.transform.SetParent(go.transform, false);
                i = lGo.AddComponent<Image>();
                i.sprite = Resources.Load<Sprite>("UI/lock");
                i.color = c;
                i.SetNativeSize();
                _locks.Add(i);
                
                
            }

            Draw();
        }
    }
    

    public List<GameObject> slides
    {
        get => _slides;
    }

    public bool isLocked(int index)
    {
        return !_locks[index].gameObject.activeSelf;
    }
    
    public void AnimateIn()
    {
        _animator.Play("AnimateIn");
    }


    public void AnimateIn(int index)
    {
        _index = index;
        Draw();
        _animator.Play("AnimateIn");
    }

    void OnNext()
    {
        _index = Mathf.FloorToInt(Mathf.Repeat((float)_index + 1, _slides.Count));
        _animator.Play("SwapImage");

        ON_SELECT_UPDATE.Dispatch(_index);
    }

    void OnPrevious()
    {
        _index = Mathf.FloorToInt(Mathf.Repeat((float)_index - 1, _slides.Count));
        _animator.Play("SwapImage");

        ON_SELECT_UPDATE.Dispatch(_index);
    }

    private void Awake()
    {
        _index = 0;

        _nextButton.ON_ANIMATED_ACTION.AddListener(OnNext);
        _prevButton.ON_ANIMATED_ACTION.AddListener(OnPrevious);
    }

    private void Draw()
    {
        for(int i = 0; i < _images.Count; i++)
        {
            _images[i].enabled = _locks[i].enabled = (i == _index);
        }
    }

    public void Unlock(int slideIndex)
    {
        Debug.Log("if unlock" + slideIndex);
        Color c = Color.white;
        c.a = 1f;
        
        _images[slideIndex].color = c;
        _locks[slideIndex].gameObject.SetActive(false);
        
    }
}
