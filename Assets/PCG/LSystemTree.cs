using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using UnityEditor;


public class TransformInfo
{
    public Vector3 position;
    public Quaternion rotation;
}
//DA DOCUMENTARE ASSOLUTAMENTE
public class LSystemTree : MonoBehaviour
{
    public bool randomIgnoreRuleModifier = true;
    public float chanceToIgnoreRule = 0.3f;
    [Range(1, 4)]
    public int iterationLimit = 1;
    [SerializeField] private GameObject Branch;
    [SerializeField] private GameObject Leaf;
    [SerializeField] private float lengthBranch = 10f;
    //[SerializeField] private float lengthLeaf = 10f;
    [SerializeField] private float angle = 30f;
    //private const float WidthBranches = 8f;
    private float widthBranch = 6f;
    private Stack<TransformInfo> transformStack;
    //private Dictionary<char, string> rules;
    private LSystemGenerator lsystem;
    public LSRule[] rules;
    [SerializeField] public string axiom = "X";
    public float variance = 10f;
    //private string currentString = string.Empty;


   /* void Start()
    {
        transformStack = new Stack<TransformInfo>();

        rules = new Dictionary<char, string>
        {
            {'X',"X[F[-X+F[+FX]][*-X+F[+FX]][/-X+F[+FX]-X]]"}, ALL F will become leaves, except for the first one that will represent the stem of the tree,
            {'X',"X[F[-X+F[+LX]][*-X+F[+LX]][/-X+F[+LX]-X]]"}
            {'F',"FF"}
        };
        Generate();
    }*/
     private void Start()
    {
        transformStack = new Stack<TransformInfo>();
        // the only test we perform on the validity of the L-system is to have the axiom not null.
        if (axiom != string.Empty)
        {
            lsystem = new LSystemGenerator(rules, iterationLimit, randomIgnoreRuleModifier, chanceToIgnoreRule);
            // we generate the sentence given axiom and rule
            var sequence = lsystem.GenerateSentence(axiom);
            StringBuilder LeavesSequence = new StringBuilder(sequence); // the L will be the leaves to better understanding the rappresentation of the F that will become leaves
            for(int i =0; i < LeavesSequence.Length; i++){
                if ((LeavesSequence[(i + 1) % LeavesSequence.Length] == 'X' ) && (LeavesSequence[i] == 'F')){
                    LeavesSequence[i] = 'L';
                }
            }
            Debug.Log(LeavesSequence);
            // we parse the final sentence
            ParseAndBuild(sequence); //graphic result based on the sequence generated by the grammar
        }
        else
        {
#           if UNITY_EDITOR
            EditorUtility.DisplayDialog("Axiom is empty!", "", "Ok");
            if (EditorApplication.isPlaying)
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }

    private void ParseAndBuild(String sequence){
        int i = 0;
        //countForTrunk non è bellissimo perchè se volessi creare un nuovo albero con nuove regole il codice non sarebbe modulare in quanto probabilmente non si 
        //applicherebbe alle nuove regole del nuovo albero.
        int countForTrunk = 2; //conto le parentesi [ nella formula di modo che fino alla quarta svenga costruito il tronco dell'albero e gli altri siano rami. 
        foreach(char c in sequence){
            switch(c)
            {   

                //NO INTERNAL LEAVES 4:
                //X[FFFFFFFFFFFFFFF[-X[FFFFFFF[-X[FFF[-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]][*-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]][/-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]]]+FFFFFFF[+L]][*-X[FFF[-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]][*-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]][/-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]]]+FFFFFFF[+L]][/-X[FFF[-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]][*-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]][/-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]]]+FFFFFFF[+L]-X[FFF[-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]][*-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]][/-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]]]]]+FFFFFFFFFFFFFFF[+L]][*-X[FFFFFFF[-X[FFF[-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]][*-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]][/-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]]]+FFFFFFF[+L]][*-X[FFF[-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]][*-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]][/-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]]]+FFFFFFF[+L]][/-X[FFF[-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]][*-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]][/-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]]]+FFFFFFF[+L]-X[FFF[-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]][*-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]][/-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]]]]]+FFFFFFFFFFFFFFF[+L]][/-X[FFFFFFF[-X[FFF[-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]][*-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]][/-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]]]+FFFFFFF[+L]][*-X[FFF[-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]][*-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]][/-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]]]+FFFFFFF[+L]][/-X[FFF[-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]][*-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]][/-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]]]+FFFFFFF[+L]-X[FFF[-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]][*-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]][/-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]]]]]+FFFFFFFFFFFFFFF[+L]-X[FFFFFFF[-X[FFF[-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]][*-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]][/-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]]]+FFFFFFF[+L]][*-X[FFF[-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]][*-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]][/-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]]]+FFFFFFF[+L]][/-X[FFF[-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]][*-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]][/-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]]]+FFFFFFF[+L]-X[FFF[-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]][*-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]][/-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]+FFF[+L]-X[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]]]]]]]
                //PERFECT RULE:[F[-X+F[+L]][*-X+F[+L]][/-X+F[+L]-X]]

                // fino alla seconda parentesi [ definisco il tronco = spessore maggiore, le successive F saranno branch
                //CAPIRE PERCHÈ LE FOGLIE NON CAMBIANO CON +, -, *, / MA SOLO LE LINEE DEI BRANCH LO FANNO.

                case 'F':
                    Vector3 initialPosision = transform.position;
                    transform.Translate(Vector3.up * lengthBranch); // capire cosa fa questo elemento

                    GameObject treeSegment = Instantiate(Branch);
                    treeSegment.transform.parent = this.gameObject.transform;   //put the leaf and the branches under the tree parent
                    treeSegment.GetComponent<LineRenderer>().SetPosition(0,initialPosision); //perchè qua funzione e nel mesh render no? capire meglio
                    treeSegment.GetComponent<LineRenderer>().SetPosition(1,transform.position);
                    treeSegment.GetComponent<LineRenderer>().startWidth = widthBranch;
                    treeSegment.GetComponent<LineRenderer>().endWidth = widthBranch;
                    break;
                    //perchè tiene traccia degli up e dei rotate quindi alla fine si trovano in posizione giusta sempre
                    // però si applica alle linee e non ai meshrender
                case 'L':  
                    Vector3 IP = new Vector3(transform.position.x, transform.position.y-4, transform.position.z); // sposto di 2 in su le y.
                    // probabilmente meglio il meshRenderer che le linee per le foglie. per il branch potrei anche tener le linee
                    GameObject l = Instantiate(Leaf);
                    
                    l.transform.position = IP;
                    //l.transform.parent = this.gameObject.transform;   //put the leaf and the branches under the tree parent <- mi bugga la posizione delle foglie
                    break;
                    //X = basic structure of the tree
                case 'X':
                    break;
                    //clockwise on z axis
               case '+':
                    transform.Rotate(Vector3.back * angle * (1 + variance / 100 * UnityEngine.Random.Range(-1f, 1f)));//capire meglio sta formula
                    break;
                    //reverse clockwise on z axis
                case '-':                                      
                    transform.Rotate(Vector3.forward * angle * (1 + variance / 100 * UnityEngine.Random.Range(-1f, 1f)));//capire meglio sta formula
                    break;
                    //moving clockwise on y axis
                case '*':
                    transform.Rotate(Vector3.up * 120 * (1 + variance / 100 * UnityEngine.Random.Range(-1f, 1f)));//capire meglio sta formula
                    break;
                    //moving reverse clockwise on y axis
                case '/':
                    transform.Rotate(Vector3.down* 120 * (1 + variance / 100 * UnityEngine.Random.Range(-1f, 1f))); //capire meglio sta formula
                    break;
                    //save turtle position
                case '[':
                    if(countForTrunk <= 0){widthBranch = 2f;}//if the trunk is ended i recompute the width to normal 
                    countForTrunk--;

                    transformStack.Push(new TransformInfo(){
                    position = transform.position,
                    rotation = transform.rotation
                });
                    break;
                    //turn back to saved position
                case ']':
                    TransformInfo ti = transformStack.Pop();
                    transform.position = ti.position;
                    transform.rotation = ti.rotation;
                    break;
                default:
                    throw new InvalidOperationException("invalid Tree operation");
            }
            i++;
        }

    }

}
