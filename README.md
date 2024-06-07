# TestUnityRoguelike2013
Sample Portions of Code from a Roguelike I was building in Unity.
This is not the whole project, just some of the files.

The goal for the project was to learn how to use Unity for the first time.
I decided to try creating a 2D sprite based game with combat similar to The Legend of Zelda: Link to the Past.  I also wanted to experiment with generating a roguelike dungeon.


1. When the game begins, roll some settings for the dungeon (things like # of rooms, bosses, etc)
2. Generate the rooms using the available space and connect them with hallways.  Include some intentional dead ends.
3. Evaluate what was created and remove some of the wasted space.  Then close all of the open rooms to simplify the playing space.
4. Generate a series of navigation points based on the connecting centerpoints of each room and hallway entrance/exit that AI enemies can use to navigate the procedurally generated dungeon to reach the player.
5. Place scenery objects (things like crates, jars, torches, etc) along with enemy spawners
6. Start the game
