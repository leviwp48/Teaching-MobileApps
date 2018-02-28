# Created By: Levi Pole
# Last Modified: 2/26/18
-- The project is named CameraExample, but I wanted it to be Vision. I couldn't figure out how to properly change the name. 

Vision
======
Vision is a game for camera fanatics who like to play a game here and there. In this game the player is asked to snap photos within certain categories. For each photo that matches the category the player receives points. Vice versa, for any photos that don't match, the player loses points. If the player obtains enough points they will advance a level. The goal of the game is to gain enough points to complete every level. 

System Design
=============

### System Requirements
1. 7.1 and above
2. Android System
3. Camera
4. Internet Connection

Usage
=====
1. Open the App
<p align="center">
     <img src="https://github.com/leviwp48/Teaching-MobileApps/blob/master/projects/project%203/source/CameraExample%20 %20Copy/Design%20Pictures/Open%20View.png"
      width="300"
      height="400"/>
</p>
     
2. Press the **Begin Game** button

3. Now you are at the main screen. 
<p align="center">
<img src="https://github.com/leviwp48/Teaching-MobileApps/blob/master/projects/project%203/source/CameraExample%20-%20Copy/Design%20Pictures/Game%20Layout.png"
     width="300"
     height="400"/>
</p>
     
Here, from top to bottom: 
        *there is a level bar (There are 5 levels), 
        *requested picture category, 
        *an imgage view, 
        *results text box,
        *and a point bar (Out of 100). 
                                                           
4. Pressing the **Take Photo** button will will take you to your default camera app. Where you may snap of a photo of anything you like.    After accepting your photo you will be taken back to the main screen. 
<p>
<img src="https://github.com/leviwp48/Teaching-MobileApps/blob/master/projects/project%203/source/CameraExample%20-%20Copy/Design%20Pictures/Accept%20Image.png"
     width="100"
     height="200"/>
</p>

5. Here you will see your image. Next hit the **Submit** button, this will analyze your image, send results to the text box, and adjust    your points and level. 
   *Adding Points 
   <img src="https://github.com/leviwp48/Teaching-MobileApps/blob/master/projects/project%203/source/CameraExample%20-%20Copy/Design%20Pictures/After%20Submit.png"
     alt="After Submit"
     style="float: left;" />
    
   *Changing Levels 
   <img src="https://github.com/leviwp48/Teaching-MobileApps/blob/master/projects/project%203/source/CameraExample%20-%20Copy/Design%20Pictures/Level%20Change.png"
     alt="Level Change"
     style="float: left; " />
   
6. Continue this process until you have passed all 5 levels and beaten the game. 
7. Finishing the game will take you to the last screen where you may replay the game or exit the game. 
 <img src="https://github.com/leviwp48/Teaching-MobileApps/blob/master/projects/project%203/source/CameraExample%20-%20Copy/Design%20Pictures/Final%20view.png"
     alt="Final"
     style="float: left; margin-right: 5px;" />

