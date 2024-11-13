using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InteractionManager
{
    private MenuWarteg menuWarteg;
    private GrabObject currentGrabObject;
    private Button buttonKonfirmasi;
    private Text textMesh;
    private Transform cam;
    private Transform grabDepan;
    private Transform grabKiri;
    private Transform targetPointMakan;
    private float direction;
    private bool canGrab;
    private bool moveToPointMakan;

    private PlayerInteract playerInteract;

    public InteractionManager(float direction, Transform cam, Text textMesh, PlayerInteract playerInteract, 
        Transform grabDepan, Transform grabKiri)
    {
        this.direction = direction;
        this.cam = cam;
        this.textMesh = textMesh;
        this.playerInteract = playerInteract;
        this.grabDepan = grabDepan;
        this.grabKiri = grabKiri;
    }

    public void PerformRaycast()
    {
        if (!cam) { Debug.LogError("Camera reference missing."); return; }

        ResetInteraction();

        Ray ray = new Ray(cam.position, cam.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, direction))
        {
            switch (hit.collider.tag)
            {
                case "PiringBersih":
                    HandleGrabObject(hit);
                    break;
                case "MenuWarteg":
                    HandleMenuWarteg(hit);
                    break;
                case "Konfirmasi":
                    HandleConfirmation(hit);
                    break;
                case "PointMakan":
                    HandleServePoint(hit);
                    break;
            }
        }
    }

    public void HandleInteractObject()
    {
        if (canGrab && currentGrabObject) TryGrab();
        else if (!canGrab && grabDepan.childCount > 0) ReleaseObject();

        if (canGrab && menuWarteg) menuWarteg.AmbilItem(5);

        buttonKonfirmasi?.onClick.Invoke();
        buttonKonfirmasi = null;

        if (moveToPointMakan) MoveObjectToServePoint();
    }

    public void SwitchGrabPosition()
    {
        Transform currentParent = grabKiri.childCount > 0 ? grabKiri : grabDepan;
        Transform targetParent = currentParent == grabDepan ? grabKiri : grabDepan;
        
        if (currentParent.childCount == 0) return;

        GrabObject objInHand = currentParent.GetChild(0).GetComponent<GrabObject>();
        objInHand.SetReference(targetParent);
        objInHand.transform.SetParent(targetParent);
    }

    private void ResetInteraction()
    {
        textMesh.text = "";
        canGrab = false;
        currentGrabObject = null;
        menuWarteg = null;
        moveToPointMakan = false;
    }

    private void HandleGrabObject(RaycastHit hit)
    {
        GrabObject grabObject = hit.collider.GetComponent<GrabObject>();
        if (grabObject == null) return;

        textMesh.text = hit.collider.name;
        canGrab = true;
        currentGrabObject = grabObject;
        grabObject.SetIsCanGrab(true);
        playerInteract.SetGrabReference(grabObject);
    }

    private void HandleMenuWarteg(RaycastHit hit)
    {
        menuWarteg = hit.collider.GetComponent<MenuWarteg>();
        if (menuWarteg == null) return;

        textMesh.text = menuWarteg.GetNameMenu();
        canGrab = true;
    }

    private void HandleConfirmation(RaycastHit hit)
    {
        buttonKonfirmasi = hit.collider.GetComponent<Button>();
        if (buttonKonfirmasi == null) return;

        textMesh.text = "Konfirmasi Pesanan!";
    }

    private void HandleServePoint(RaycastHit hit)
    {
        if (grabDepan.childCount == 0) return;

        targetPointMakan = hit.collider.transform;
        moveToPointMakan = true;
        textMesh.text = "Sajikan Makanan!";
    }

    private void TryGrab()
    {
        if (grabDepan.childCount > 0) return;

        currentGrabObject.SetReference(grabDepan);
        currentGrabObject.ToggleGrab();
    }

    private void ReleaseObject()
    {
        GrabObject grabObject = grabDepan.GetChild(0).GetComponent<GrabObject>();
        grabObject.ToggleGrab();
        grabObject.SetReference(null);

        if (grabObject == currentGrabObject) currentGrabObject = null;
    }

    private void MoveObjectToServePoint()
    {
        GrabObject grabObject = grabDepan.GetChild(0).GetComponent<GrabObject>();

        grabObject.SetReference(targetPointMakan);
        grabObject.transform.position = targetPointMakan.position;
        grabObject.transform.SetParent(null);
        grabObject.gameObject.tag = "Untagged";
        
        targetPointMakan.parent.GetChild(1).gameObject.SetActive(true);
        targetPointMakan.gameObject.SetActive(false);
        
        moveToPointMakan = false;
    }

    public void SetPlayerInteract(PlayerInteract playerInteract) => this.playerInteract = playerInteract;
}
