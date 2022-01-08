using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TreeSpawnManager : MonoBehaviour
{
    //Need to find a method to spawn trees onto the perlin terrain, Maybe on top of the hills?
    [Range(0, 20)]
    [SerializeField] private int HowManyTrees;
    [SerializeField] private List<GameObject> Tree;
    
    // Start is called before the first frame update
    void Start()
    {
       SpawnTrees();
    }



    //Genereate random Vector3, consider that the ground is 500x500 meters;

    //can happen that 2 trees are instantiated at same position, BUT IS THIS APROBLEM?  in a forest the trees are all nearby, if I want to create a forest like effect
    // the trees must be near. It also can be easily resolved by putting a collider inside every tree, the last spawned tree that doesn't hit the terrain will be respawned in a different position.
    Vector3 GeneratePosition()
    {
        //240 because I don't want them to be placed in the edges
        int x,y,z;
        x = Random.Range(-240, 240);
        y = 200;    //The point is the highest possible, so that the trees don't touch the ground, from this point I raycast the ground and find the closest pependicular point in order to spawn the trees;
        z = Random.Range(-240, 240);
        return new Vector3(x,y,z);
    }

    private void SpawnTrees(){
        int RandomSpawn = 0;
        int attemps; //attempts for the tree to find the terrain.
        for(int i = 0; i < HowManyTrees; i++){
            attemps = 0;
            RandomSpawn = Random.Range(0, Tree.Count); // could be a number in which the list is pointing to null since another programmer didn't assign the gameObject to spawn.
            //let's make it more clear to the user:
            try{
                Vector3 start = GeneratePosition();
                GameObject tree = Instantiate(Tree[RandomSpawn], start, Quaternion.identity);// here are spawned but the y needs to be modified correctly, so that trees will appear on the ground and not floating
                while(Physics.Raycast(tree.transform.position, transform.TransformDirection(Vector3.down), out RaycastHit rayInfo ) && attemps <= 5){
                    if(rayInfo.transform.tag == "Terrain"){
                        tree.transform.position = rayInfo.point; //move position where it hits the point on the terrain
                        //Debug.Log(rayInfo.collider.gameObject.tag);
                        break;
                    }else{ //if the spawned tree can't find terrain it's usefull to respawn it in another location and than try again to find the terrain.
                        tree.transform.position = GeneratePosition();
                        attemps += 1;
                    }
                    // if after 5 attemps the tree couldnt find the terrain, destroy the gameobject.
                    if(attemps >=5){
                        Destroy(tree);
                    }
                       
                }

            }catch(UnassignedReferenceException){
                EditorUtility.DisplayDialog ("ERROR", "You have to assign correctly the trees on the list of the TreeManager", "Ok");
                return;
            }
        }
    }

}
