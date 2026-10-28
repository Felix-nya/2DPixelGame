using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Hero))]
public class HeroPointer : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public event Action<Hero, Vector2> BeginDrag;
    public event Action<Hero, Vector2> Drag;
    public event Action<Hero, Vector2> EndDrag;

    private Hero hero;

    private void Awake()
    {
        hero = GetComponent<Hero>();
    }

    public void OnBeginDrag(PointerEventData eventData) => BeginDrag?.Invoke(hero, ToWorld(eventData));
    public void OnDrag(PointerEventData eventData) => Drag?.Invoke(hero, ToWorld(eventData));
    public void OnEndDrag(PointerEventData eventData) => EndDrag?.Invoke(hero, ToWorld(eventData));

    private static Vector2 ToWorld(PointerEventData eventData)
    {
        Camera cam = eventData.pressEventCamera != null ? eventData.pressEventCamera : Camera.main;
        Vector3 world = cam.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, 0f));
        return new Vector2(world.x, world.y);
    }
}