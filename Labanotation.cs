using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SimpleJSON;
using System.Linq;

public class Labanotation : MonoBehaviour

{
    [Header("Rotating body parts")]
    [SerializeField] private Transform leftArm;
    [SerializeField] private Transform rightArm;
    [SerializeField] private Transform leftLeg;
    [SerializeField] private Transform rightLeg;

    [Header("Rotation parameters")]
    //[SerializeField] private float moveDuration;
    [SerializeField] private float degrees;
    [SerializeField] private float completionSteps;

    private float counterTime = 0;
    private float totalCounterTime = 0;

    private MovementClass movements;

    List<MovementClass> movementsList;
    List<MovementClass> chosenMovementsList;

    private Vector3 rightLegAnchorPosition;
    private Vector3 leftLegAnchorPosition;
    private Vector3 rightArmAnchorPosition;
    private Vector3 leftArmAnchorPosition;


    // Start is called before the first frame update
    void Start()
    {
        //prepei na arxikopoiw tis listes prin kanei parse to Json k oxi prin!! simantiko ..
        movementsList = new List<MovementClass>();
        chosenMovementsList = new List<MovementClass>();

        parseJsonFile();

        bodysPartAnchorPositionDefinition();
    }

    private void bodysPartAnchorPositionDefinition()
    {
        rightLegAnchorPosition = rightLeg.transform.rotation.eulerAngles;
        leftLegAnchorPosition = leftLeg.transform.rotation.eulerAngles;
        rightArmAnchorPosition = rightArm.transform.rotation.eulerAngles;
        leftArmAnchorPosition = leftArm.transform.rotation.eulerAngles;
    }

    private void parseJsonFile()
    {
        string jsonString = File.ReadAllText("Assets/Labans/laban_sample.json");
        JSONNode movement = JSON.Parse(jsonString)["generated_motion_sample"]["Movement"];
        // m autin tin grammi pairnw sigkekrimeno periexomeno pou vrisketai sto sigkekrimeno index
        /*        var v = JSON.Parse(jsonString)["generated_motion_sample"]["Movement"]["Starting Position"][0]["Direction"];
                Debug.Log(v);*/

        //prepei na kanw clear tis listes etsi wste se neo json arxeio na min krataei ta proigoumena
        movementsList.Clear();
        chosenMovementsList.Clear();


        // a position is a collection for body part positions
        foreach (JSONNode position in movement)
        {
            foreach (JSONNode bodyPartPosition in position)
            {
                movements = new MovementClass();

                movements.BodyPart = bodyPartPosition["Body Part"];
                movements.Direction = bodyPartPosition["Direction"];
                movements.Level = bodyPartPosition["Level"];
                movements.Start = bodyPartPosition["Start"].AsFloat;
                movements.End = bodyPartPosition["End"].AsFloat;

                movementsList.Add(movements);
            }

            //enallaktikos tropos me linq gia na kanw debug log ta items se mia lsita
            //leftLegMovementsList.ForEach((item) => Debug.Log(item.ToString));
        }

        //psaxnw mesa stin lista ola ta items pou perilamvanoun to string left leg
        //foreach (MovementClass item in listOfMovements.FindAll(match => match.BodyPart == "Left Leg"))    
        //{
        //}
    }


    IEnumerator moveBodysPart(Transform bodyPart, Vector3 bodysPartNewPosition, float animationTime, float steps)
    {
        if (animationTime == 0f)
        {
            bodyPart.transform.rotation = Quaternion.Euler(bodysPartNewPosition);
        }
        else if (animationTime > 0)
        {
            Quaternion bodysPartPrevPosition = bodyPart.transform.rotation;

            float time = 0f;
            float deltaStep = 1 / steps;
            float waitValue = animationTime / steps;

            while (time < 1f)
            {
                bodyPart.transform.rotation = Quaternion.Lerp(bodysPartPrevPosition, Quaternion.Euler(bodysPartNewPosition), time);

                time += deltaStep;

                yield return new WaitForSeconds(waitValue);
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        List<MovementClass> chosenMovementsList = checkIfEachMovementsStartTimeEqualsToCounterTimeAndAddMovementToNewList();

        foreach (var movement in chosenMovementsList)
        {
            (Transform bodyPart, Vector3 bodyPartAnchorPosition) = bodysPartAndTransformDefinition(movement);
            int movementsLevel = movementsLevelDefinition(movement);
            float rotationDegrees = rotationDegreesCalculation(degrees, movementsLevel);
            Vector3 movementsDirection = movementsDirectionCalculation(movement, rotationDegrees);
            Vector3 bodysPartNewPosition = bodysPartNewPositionCalculation(bodyPart, bodyPartAnchorPosition, movementsDirection);
            float animationTime = movement.End - movement.Start;

            StartCoroutine(moveBodysPart(bodyPart, bodysPartNewPosition, animationTime, completionSteps));
        }

        //Debug.Log(chosenMovementsList.Count());

        //for (int i = 0; i < chosenMovementsList.Count(); i++)
        //{
        //    Debug.Log(i);
        //}
    }

    private Vector3 bodysPartNewPositionCalculation(Transform bodyPart, Vector3 bodyPartAnchorPosition, Vector3 movementsDirection)
    {
        Vector3 to;
        if (bodyPart == rightLeg || bodyPart == rightArm)
        {
            to = bodyPartAnchorPosition - movementsDirection;
        }
        else
        {
            to = bodyPartAnchorPosition + movementsDirection;
        }
        return to;
    }

    private List<MovementClass> checkIfEachMovementsStartTimeEqualsToCounterTimeAndAddMovementToNewList()
    {
        //time.deltaTime einai o pragmatikos xronos gi auto ton prosthetw sto counter
        counterTime += Time.deltaTime;

        chosenMovementsList = new List<MovementClass>();

        if (counterTime == 0 || counterTime >= 0.5)
        {
            float startTime;
            foreach (var movement in movementsList)
            {
                //print(movement.Start);
                startTime = movement.Start;
                //print((startTime == totalCounterTime) + " "+ startTime +" "+ totalCounterTime);
                if (startTime == totalCounterTime)
                {
                    chosenMovementsList.Add(movement);
                    //movementsList.Remove(movement);
                }
                else
                {
                    if (startTime > totalCounterTime)
                    {
                        //continue;
                        break;
                    }
                }
            }
            totalCounterTime += 0.5f;
            counterTime = 0;
        }
        return chosenMovementsList;
    }


    private (Transform, Vector3) bodysPartAndTransformDefinition(MovementClass movement)
    {
        Transform bodyPart;
        Vector3 anchor;

        switch (movement.BodyPart)
        {
            case "Left Arm":
                bodyPart = leftArm;
                anchor = leftArmAnchorPosition;
                break;
            case "Right Arm":
                bodyPart = rightArm;
                anchor = rightArmAnchorPosition;
                break;
            case "Left Support":
                bodyPart = leftLeg;
                anchor = leftLegAnchorPosition;
                break;
            case "Right Support":
                bodyPart = rightLeg;
                anchor = rightLegAnchorPosition;
                break;
            default:
                bodyPart = null;
                anchor = Vector3.zero;
                print("Wrong body part tag given");
                break;
        }
        return (bodyPart, anchor);
    }

    //Calculate rotation vector given direction and level for a specific body part
    private Vector3 movementsDirectionCalculation(MovementClass movement, float degreesToRotate)
    {
        Vector3 direction;
        switch (movement.Direction)
        {
            case "Place":
                // needs to be changed
                direction = new Vector3(0, 0, 0);
                break;
            case "Forward":
                direction = new Vector3(-degreesToRotate, 0, 0);
                break;
            case "Backwards":
                direction = new Vector3(degreesToRotate, 0, 0);
                break;
            case "Left":
                direction = new Vector3(0, 0, -degreesToRotate);
                break;
            case "Right":
                direction = new Vector3(0, 0, degreesToRotate);
                break;
            case "Left Forward":
                direction = new Vector3(degreesToRotate, 0, -degreesToRotate);
                break;
            case "Right Forward":
                direction = new Vector3(-degreesToRotate, 0, degreesToRotate);
                break;
            case "Left Backwards":
                direction = new Vector3(-degreesToRotate, 0, -degreesToRotate);
                break;
            case "Right Backwards":
                direction = new Vector3(degreesToRotate, 0, degreesToRotate);
                break;
            default:
                direction = new Vector3(0, 0, 0);
                print("Wrong direction tag given");
                break;
        }
        return direction;
    }

    //Calculate degrees to rotate for a specific body part
    private float rotationDegreesCalculation(float degrees, int level)
    {
        float rotation;
        rotation = degrees * level;
        return rotation;
    }

    //Define level of movement for a specific body part
    private int movementsLevelDefinition(MovementClass movement)
    {
        int level;
        switch (movement.Level)
        {
            case "Low":
                level = 0;
                break;
            case "Mid":
                level = 1;
                break;
            case "High":
                level = 2;
                break;
            default:
                level = 0;
                print("Wrong level tag given");
                break;
        }
        return level;
    }
}

public class MovementClass
{
    private string bodyPart;
    private string direction;
    private string level;
    private float start;
    private float end;

    public MovementClass()
    { }

    public string BodyPart { get => bodyPart; set => bodyPart = value; }
    public string Direction { get => direction; set => direction = value; }
    public string Level { get => level; set => level = value; }
    public float Start { get => start; set => start = value; }
    public float End { get => end; set => end = value; }
}
