# ContextSteering Unity
A research project regarding "context steering behaviors" for use in games with a practical implemention in the Unity game engine.

## Introduction
Steering behaviors are movement algorithms that determine where an AI agent should be next.
These algorithms use basic information about the AI agent (current position, velocity, direction, ...) and the world to make a decision on where to go next.
The steering behavior will calculate a direction vector to adjust the movement of the AI agent.

These simple steeringbehaviors can then be combined in combined steering behaviors the create more complex movement and make the agent seem more intelligent.

## The need for context steering
The need for context steering arises when players are able to inspect individual AI agents and observe them closely avoiding collision with other agents and with the static world. The more steering behaviors are combined the harder it will become for the developer to tune the parameters of each and every one of those steering behaviors to achieve the behavior needed. This could also possibly mean that the behavior components itself will grow in size and become tightly coupled. These tightly coupled behaviors can then cause problems in terms of maintainability of the codebase.

Context steering combines small context behaviors that can be combinded together without tightly coupling them.
## Context steering overview
Think of context steering as a steering behavior that wants to go in a certain amount of directions equally divided over a circle.
If it was just this, the agent would stand still because all directions have an equal length thus they all apply the same amount of force to the agent negating eachother. We alter the force applied by these directions by the desire of the behavior to go in a certain direction. 
These scalar values of how desired or undesired a certain direction is are stored in context maps.
![Directions](https://user-images.githubusercontent.com/41028126/151255179-698c4187-62a6-4f96-9a4c-20d895df3d42.png)
    image from: Game AI 2 Chapter 18: Behavior-Driven steering at the macro scale
### Context maps
Each context behavior has 2 context maps, an interest context map and a danger context map. The context steeringbehavior uses the interst map to represent its desire to go into a certain direction while the danger map represents the oposite.

For example a chase or seek context behavior will fill the slots of the interest map with higher scaler values relative the amount the corresponding direction of the slot is pointing in the same direction as the direction vector to the target of the chase behavior (think Dot product).

An avoid context steering behavior will do the exact opposite this behavior.
Each slot of the danger map again corresponds to a direction the agent can move in and the value in the slot itself represents the behaviors desire to NOT go into that direction.

Keep in mind that there should always be an equal amount of slots in both the interest map and danger map as there are directions the agent can move in.
![context map](https://user-images.githubusercontent.com/41028126/151255261-b1766052-473b-4a7b-b0f2-edc07f100a65.png)
    image from: Game AI 2 Chapter 18: Behavior-Driven steering at the macro scale
### Context Merger
The context merger will gather all interest and danger maps from all context behaviors active on the AI agent and merge them together to get to a final direction vector result to move the agent with.

#### How the context maps are merged

First all context maps are gathered by the context merger to build a final interest and danger map to calculate the final direction.

For both the interest and the danger map, the merger loops over all slots and picks the highest value it can find for that particular slot from all its corresponding maps (interest maps for final interest map, danger maps for final danger map). We could also calculate the average to have for example even less of a desire to move to a spot where 2 avoid targets are but this is unneccesary because the avoidance of the first obstacle will already keep us safe from the obstacle behind it.

When we then have calculated both the final interest and danger map we subtract each interest slot of our final interest map by its corresponding slot in the final danger map. This way the interests towards our goal are altered if there is and obstacle on our path.
![parsing context map](https://user-images.githubusercontent.com/41028126/151255362-ae85c3dd-fd2c-4733-8b7a-e6e608562433.png)
    image from: Game AI 2 Chapter 18: Behavior-Driven steering at the macro scale
## Implementation
This section will describe the implemetion of context steering in the unity application.
### Context Merger
#### Memeber variables
- m_MapResolution: Determines the amount of directions used for calculating the final direction, also determines size of the interest and danger maps
- m_MovementSpeed: Movement speed of the agent
- m_Behaviors: Array of all Context behaviors associated to this agent
- m_Directions: list of direction vectors
- m_InterestMap: final interest map
- m_DangerMap: final danger map
````
    [SerializeField] private int m_MapResolution;
    [SerializeField] private float m_MovementSpeed;

    [SerializeField] private BaseContextBehavior[] m_Behaviors;

    private List<Vector2> m_Directions;
    private List<float> m_InterestMap;
    private List<float> m_DangerMap;
    Rigidbody2D m_Rigibody2D;
````

#### Initializing the directions
This function initializes all directions equally divided on a circle these are the directions that will be altered by the desires from the context maps
````
    void InitializeDirections()
    {
        float twoPi = Mathf.PI * 2;
        float directionInterval = twoPi / m_MapResolution;
        for (int i = 0; i < m_MapResolution; i++)
        {
            float currentAngle = i * directionInterval;

            m_Directions.Add(new Vector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle)));

        }

        m_InterestMap = new List<float>(new float[m_Directions.Count]);
        m_DangerMap = new List<float>(new float[m_Directions.Count]);

    }
````



#### Merging maps
First we gather all interest maps from all behaviors.
````
       List<List<float>> interestMaps = new List<List<float>>();
        List<List<float>> dangerMaps = new List<List<float>>();

        foreach(BaseContextBehavior behavior in m_Behaviors)
        {
            interestMaps.Add(behavior.GetInterestMap(gameObject.transform.position, ref m_Directions));
            dangerMaps.Add(behavior.GetDangerMap(gameObject.transform.position, ref m_Directions));
        }
````
Then we calculate the biggest value for a slot ranging all gathered interest maps. We do this for each direction (for each slot in the interest map)
````
        for (int i = 0; i < m_InterestMap.Count; i++)
        {
            float biggestInterestForThisSlot = 0;
            for (int k = 0; k < interestMaps.Count; k++)
            {
                if (interestMaps[k][i] > biggestInterestForThisSlot)
                    biggestInterestForThisSlot = interestMaps[k][i];
            }
            
            m_InterestMap[i] = biggestInterestForThisSlot;
        }
````
Then we do the exact same for all the danger maps.
````
        for (int i = 0; i < m_DangerMap.Count; i++)
        {
            float biggestInterestForThisSlot = 0;
            for (int k = 0; k < dangerMaps.Count; k++)
            {
                if (dangerMaps[k][i] > biggestInterestForThisSlot)
                    biggestInterestForThisSlot = dangerMaps[k][i];
            }

            m_DangerMap[i] = biggestInterestForThisSlot;
        }
````

#### Calculating final interest map
We calculate the final interest map by subtracting our current interest map by the values of our danger map.
````
        for (int i = 0; i < m_DangerMap.Count; i++)
        {
            m_InterestMap[i] -= m_DangerMap[i];
        }
````
Then finally we search for the biggest desire in our interest map and use that direction as a movement direction.
````
        float biggestInterest = Mathf.Max(m_InterestMap.ToArray());
        int indexOfBiggestInterest = m_InterestMap.FindIndex(x => (x == biggestInterest));
        m_Rigibody2D.AddForce(m_Directions[indexOfBiggestInterest] * m_MovementSpeed * Time.deltaTime);
````

For a Directional context steering behavior that for example always want to go forward it will be neccessary to alter the rotation of the agent to the movement direction.
````
        Vector2 lookdirection = (m_Rigibody2D.velocity + agentPosition) - agentPosition;
        float angle = Mathf.Atan2(lookdirection.y, lookdirection.x) * Mathf.Rad2Deg - 90.0f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
````

### Base context behavior
The base context behavior is the base class where all other context steering behaviors are derived from. This abstract class specifies the GetInterestMap() and GetDangerMap() functions. These functions are to be implemented by derriving sub classes to create and return the interest and dangermap respectivly.
````
    abstract public List<float> GetInterestMap(Vector2 agentPostion, ref List<Vector2> directions);
    abstract public List<float> GetDangerMap(Vector2 agentPostion, ref List<Vector2> directions);
````
### Chase context behavior
This context behavior will calculate the interest and danger map for chasing down a target.
Important to note is that the danger map is filled with zeros because the chase behavior only has desires towards a target direction.
#### Member variables
m_MaxChaseDistance: Max distance to chase down a target
m_ChaseTargets: Array of targets to chase
````
    [SerializeField] private float m_MaxChaseDistance;
    private GameObject[] m_ChaseTargets;
````
#### GetInterestMap
Calculating the interest map based on the distance that the agent is from the target. How closer the agent is the higher the interest will be.
````
    public override List<float> GetInterestMap(Vector2 agentPosition, ref List<Vector2> directions)
    {
        m_InterestMap = new List<float>(new float[directions.Count]);
        foreach(GameObject target in m_ChaseTargets)
        {
            Vector2 targetPos = target.transform.position;
            Vector2 toTarget = targetPos - agentPosition;

            if (toTarget.magnitude > m_MaxChaseDistance)
                continue;

            for (int i = 0; i < directions.Count; i++)
            {
                float interestAmount = Vector2.Dot(toTarget, directions[i]) / toTarget.magnitude;
                Debug.Log(i);

                if (m_CenterBetweenTargets)
                {
                    m_InterestMap[i] = m_InterestMap[i] + interestAmount;
                }
                else
                {
                    if(interestAmount > m_InterestMap[i])
                    {
                        m_InterestMap[i] = interestAmount;
                    }
                }
            }
        }
        return m_InterestMap;
    }
````
### Avoid context behavior
The avoid context behavior does the exact oposite from the chase behavior. Instead of filling the interest map, this behavior will fill its danger map to make the context merger know that it doesn't want to go into a certain direction.
#### Member variables
m_MaxAvoidDistance: max distance to avoid a far away target to influence the dangermap
m_AvoidTarget: array of targets that the behavior wants to avoid
````
    [SerializeField] private float m_MaxAvoidDistance;
    private GameObject[] m_AvoidTargets;
````

#### GetDangerMap
````
public override List<float> GetDangerMap(Vector2 agentPosition, ref List<Vector2> directions)
    {

        m_DangerMap = new List<float>(new float[directions.Count]);

        foreach (GameObject avoidTarget in m_AvoidTargets)
        {
            Vector2 targetPos = avoidTarget.transform.position;
            Vector2 toTarget = targetPos - agentPosition;

            if (toTarget.magnitude > m_MaxAvoidDistance)
                continue;

            for (int i = 0; i < directions.Count; i++)
            {
                float dangerAmount = Vector2.Dot(toTarget, directions[i]) / toTarget.magnitude;
                Debug.Log(i);


                if (dangerAmount > m_DangerMap[i])
                {
                    m_DangerMap[i] = dangerAmount;
                }
            }
        }

        return m_DangerMap;
    }
````

### Directional context behavior
The directional context behavior is a very simple behavior that fills the interest context map to just go in the agents forward direction.
#### GetInterestMap
````
    public override List<float> GetInterestMap(Vector2 agentPostion, ref List<Vector2> directions)
    {
        m_InterestMap = new List<float>(new float[directions.Count]);

        for (int i = 0; i < directions.Count; i++)
        {
            m_InterestMap[i] = Vector2.Dot(gameObject.transform.up * 0.8f, directions[i]);
        }
        
        return m_InterestMap;
    }
````
## Result
### ContextSteering behaviour using Chase and avoid
In this example you can see the "Chase context behavior" in action together with the "avoid context behavior".
By combining these we can get a simple pathfinding agent that finds the target through a simple corridor with obstacles.
![ContextSteeringPathFinding](https://user-images.githubusercontent.com/41028126/151200242-e4261247-d152-46fb-8299-14b755f4c060.gif)

### ContextSteering behaviour Directional steering and avoid
In this example you can see the "directional context steering" and the "avoid context steering" in action.
This is a really nice example of the power of context steering. If you look at the implementation of these behaviors the are decoupled from eachother. The only thing the directional behavior needs to worry about is showing its desire to go forward. And the avoid only shows its unintend of being close to "avoid targets". But when these are combined we already get a seemingly smart AI agent that can traverse simple racetracks.
![ContextSteeringRacing](https://user-images.githubusercontent.com/41028126/151201575-8f0ae3fe-27a4-4245-b2f1-cb11f022bc0a.gif)

## Conclusion
Context steering behaviors have a simple system of decoupled behaviors that is easy to maintain and implement while providing decently impressive results even with a basic implementation. Though one should first consider wheter this method of steering behaviors is a good fit for the game they are making.
### Usage in games
Context steering has been used in a variaty of games including but not limmited to.
- F1 2011 by CodeMasters
## Sources
- Game AI 2 Chapter 18: Behavior-Driven steering at the macro scale
    by Andrew Fray
- AI Context behaviors
    https://jameskeats.com/portfolio/contextbhvr.html
- Context behaviours know how to share
    https://andrewfray.wordpress.com/2013/03/26/context-behaviours-know-how-to-share/
- Racing AI with Context Steering - Andrew Fray
    https://www.youtube.com/watch?v=2fg-th5cTpg
