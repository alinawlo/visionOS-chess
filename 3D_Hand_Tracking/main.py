import cv2
from cvzone.HandTrackingModule import HandDetector
import socket

# Parameters
width, height = 1280, 720

# Webcam
cap = cv2.VideoCapture(0)
cap.set(3, width)
cap.set(4, height)

# Hand Detector
detector = HandDetector(maxHands=1, detectionCon=0.8)

# Communication
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
serverAddressPort = ("localhost", 5032)

while True:
    # Get image
    success, img = cap.read()
    
    # Hands
    hands, img = detector.findHands(img)

    data = []
    # Landmark values - (x,y,z) * 21
    if(hands):
        # Get first hand
        hand = hands[0]
        # Get landmark list
        lmList = hand['lmList']
        for lm in lmList:
            data.extend([lm[0], height-lm[1], lm[2]])
        sock.sendto(str.encode(str(data)), serverAddressPort)

    cv2.imshow("Image", img)
    cv2.waitKey(1)