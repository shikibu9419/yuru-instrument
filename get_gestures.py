import cv2
import mediapipe as mp
import math
import socket

host = '127.0.0.1'
port = 8888

client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
client.connect((host, port))

mp_drawing = mp.solutions.drawing_utils
mp_drawing_styles = mp.solutions.drawing_styles
mp_hands = mp.solutions.hands

# For webcam input:
cap = cv2.VideoCapture(0)
with mp_hands.Hands(
    max_num_hands=1,
    min_detection_confidence=0.7,
    min_tracking_confidence=0.7) as hands:
  while cap.isOpened():
    success, image = cap.read()
    if not success:
      print("Ignoring empty camera frame.")
      # If loading a video, use 'break' instead of 'continue'.
      continue

    # Flip the image horizontally for a later selfie-view display, and convert
    # the BGR image to RGB.
    image = cv2.cvtColor(cv2.flip(image, 1), cv2.COLOR_BGR2RGB)
    # To improve performance, optionally mark the image as not writeable to
    # pass by reference.
    image.flags.writeable = False
    results = hands.process(image)

    # Draw the hand annotations on the image.
    image.flags.writeable = True
    image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)
    height, width, _ = image.shape
    if results.multi_hand_landmarks:
      for hand_landmarks in results.multi_hand_landmarks:
        mp_drawing.draw_landmarks(
            image,
            hand_landmarks,
            mp_hands.HAND_CONNECTIONS,
            mp_drawing_styles.get_default_hand_landmarks_style(),
            mp_drawing_styles.get_default_hand_connections_style())

        index_tip_coods = hand_landmarks.landmark[mp_hands.HandLandmark.INDEX_FINGER_TIP]
        thumb_tip_coods = hand_landmarks.landmark[mp_hands.HandLandmark.THUMB_TIP]

        # print(index_tip_coods, middle_tip_coods)
        cv2.rectangle(
          image,
          (int(thumb_tip_coods.x * width), int(thumb_tip_coods.y * height)),
          (int(index_tip_coods.x * width), int(index_tip_coods.y * height)),
          (0, 255, 0),
          2
        )

        distance = math.sqrt(math.pow(thumb_tip_coods.x - index_tip_coods.x, 2) + math.pow(thumb_tip_coods.y - index_tip_coods.y, 2))

        print('send:', distance)
        client.send(str(distance).encode())
        print(client.recv(4096))

    cv2.imshow('MediaPipe Hands', image)
    if cv2.waitKey(5) & 0xFF == 27:
      break

cap.release()