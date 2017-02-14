﻿using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System;
using System.Net;
using System.Text;
using System.Collections.Generic;

public class udpsocket : MonoBehaviour
{
    public int Port;
    UdpClient Client;
    public GameObject eyepointer;
    public Vector2 LastEyeCoordinate;
    public GameObject camera;

    createMarker markerScript;
    Bubble bubbleScript;
    RectTransform rect;

	float screenDiagonal;

	public List<Vector2> processingList; 
    
	void Start()
    {
		processingList = new List<Vector2> ();
        markerScript = camera.GetComponent<createMarker>();
        bubbleScript = camera.GetComponent<Bubble>();
        rect = eyepointer.GetComponent<RectTransform>();

		screenDiagonal = Vector2.Distance (Vector2.zero, markerScript.newScreen);
    }

    void Update()
    {
        //bubbleScript.calcBubble(LastEyeCoordinate);

		//filter out when list gets 30 points
		if (processingList.Count >= 10) {
            List<Vector2> processingList_snapshot = processingList;
            processingList = new List<Vector2>();
			Vector2 currentGaze = FilterGazeCoordinates (processingList_snapshot, false);

			if (NotNoise (currentGaze)) {
				LastEyeCoordinate = currentGaze;
			}
		}

        rect.anchoredPosition = LastEyeCoordinate;
    }
    /*
     private void recv(IAsyncResult res)
    {
	
		if (Encoding.UTF8.GetString (received).Length > 0) {
			//parse coordinates
			processingList.AddRange(ExtractGazeCoordinates (false, Encoding.UTF8.GetString (received)));
		} else {
			eyepointer.GetComponent<RectTransform>().anchoredPosition = new Vector2(((float)Input.mousePosition.x - (Screen.width / 2)), (float)Input.mousePosition.y - (Screen.height / 2));
			processingList = new List<Vector2> (); 
		}
    }
    */

	Boolean NotNoise(Vector2 currentGaze){
		if (LastEyeCoordinate == null)
			return true;
		
		// distance of the currentGaze from previous reading
		float distance = Vector2.Distance(currentGaze, LastEyeCoordinate);

		if (screenDiagonal == 0) {
			screenDiagonal = Vector2.Distance (Vector2.zero, markerScript.newScreen);
		}

		return (distance / screenDiagonal) > 0.05f;
	}

	Vector2 FilterGazeCoordinates(List<Vector2> processingList, Boolean flip_y){

        List<Vector2> ModifiedCoordinates = new List<Vector2>();
        foreach (Vector2 point in processingList)
        {
            float x = point.x;
            float y = point.y;
            Debug.Log(markerScript.newScreen);
            x *= markerScript.newScreen.x;
            if (flip_y) { y = 1 - y; }
            y *= markerScript.newScreen.y;

            ModifiedCoordinates.Add(new Vector2(((float)x - (markerScript.newScreen.x / 2)), (float)y - (markerScript.newScreen.y / 2)));
        }

		Double x_sum = 0;
		Double y_sum = 0;

		foreach (Vector2 point in ModifiedCoordinates) {
            x_sum += point.x;
			y_sum += point.y;
		}

		Vector2 centeroid = new Vector2((float) x_sum / ModifiedCoordinates.Count, (float) y_sum / ModifiedCoordinates.Count);

		List<Vector3> DistanceFromCenteroid = new List<Vector3> ();

		foreach (Vector2 point in ModifiedCoordinates) {
			DistanceFromCenteroid.Add (new Vector3 (point.x, point.y, Vector2.Distance(point, centeroid)));
		}

		DistanceFromCenteroid.Sort((a, b) => a.z.CompareTo(b.z));
		List<Vector3> trimmedList = DistanceFromCenteroid.GetRange (0, (int)(ModifiedCoordinates.Count * 0.75));

		x_sum = 0;
		y_sum = 0;
		foreach (Vector3 point in trimmedList) {
			x_sum += point.x;
			y_sum += point.y;
		}
		return new Vector2((float) x_sum / trimmedList.Count, (float)  y_sum / trimmedList.Count);
	}

}
