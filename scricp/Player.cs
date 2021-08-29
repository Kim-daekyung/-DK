using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour,ObjInterface
{
    public static string gType = "Player";
    public ObjBase mOb;

    private Vector3 vector;
    
    public float speed;
    private bool jump = false;
    public int jumpcount = 2;
    public bool canmove = true;
    private bool attacktime = true;
    public int attackturn;
    public float f1jumping = 0f;
    public bool f1jump = false;
   
  
    public Rigidbody2D rigid;
    
    

    public Collider2D collie;
    public int dead = 0;

    public bool wait = false;
    private bool ground = false;
    private int  fly = 0;
    private float waitTime = 0;

    private bool jumpStop = false;


    public F2Ground f2G;
    public float jumppower = 0.04f;
    public float endjump = -0.5f;
    public bool mbJumpUp = false;
    public bool mbJumpDown = false;
    
    public float maxJump;
    public bool f2jump=false;
    public Vector3 oldPos;
    public bool doubleJ = false;

    public bool jumping=false;



    //public void OnAttack(MonoBehaviour mb)
    //{
    //    if (mb.name.Equals("Emy") || mb.name.Equals("Emy(Clone)"))
    //    {
    //        Debug.Log(" " + mb.name);
    //        Emy1 es = (Emy1)mb;
    //        es.mOb.curhp -= mOb.attackp;
    //        Debug.Log(mb.name + " Hit " + mOb.attackp);
    //        if (es.mOb.curhp <= 0)
    //        {

    //            Debug.Log("Dead");

    //            GameObject.Destroy(es.gameObject);


    //            Gv.gThis.mOm.Remove(es.mOb);


    //        }

    //    }



    //}
    //public Vector3 getPos()
    //{
    //    return this.transform.position;
    //}

    // Start is called before the first frame update
    void Start()
    {
        mOb = new ObjBase();
        mOb.mMb = this;
        mOb.mType = Player.gType;
        wait = false;

        f2G = FindObjectOfType<F2Ground>();
        Gv.gThis.mOm.Add(this.mOb);
        

        
        //Hitbox 
        mOb.HitboxR = this.transform.Find("HItBoxRight").gameObject;
        mOb.HitboxR.SetActive(false);
        mOb.HitboxL = this.transform.Find("HItBoxLeft").gameObject;
        mOb.HitboxL.SetActive(false);
        mOb.righ= "Right";


       
        transform.position = new Vector3(transform.position.x,f1jumping,transform.position.z);
        f1jumping = -0.52f;
        f1jump = true;



    }

    // Update is called once per frame
    void Update()
    {
        oldPos = mOb.getPos();
        endjump = this.transform.position.y;
        Om om = Gv.gThis.mOm;
        mOb.time += Time.deltaTime;

        if (mbFalldown && mbJumpUp)
        {
            mbFalldown = false;
        }
       
        if (Input.GetKey(KeyCode.DownArrow)&&Input.GetKey(KeyCode.Space) &&ground==true&&fly==0)
        {  
            lowerJump();
            fly++;
            jumpcount = 1;
            
           

        }
       
        if (Input.GetButtonDown("Jump") && jumping == false)
        {
            
            maxJump = transform.position.y + 2f;
            Debug.Log(endjump);
            

            if (endjump < maxJump && mbJumpDown == false)
            {

                Debug.Log("mbDo=false");
                mbJumpUp = true;
                jumping = true;
               


            }
            if(Input.GetButtonDown("Jump")&&doubleJ==false)
            {
                maxJump = transform.position.y + 2f;
                doubleJ = true;

            }






        }



        //Debug.Log("" + HitboxR.transform.position);

        if (Input.GetKeyDown(KeyCode.Q))
        {   
            GameObject hitbox = mOb.GetHitBox();
            List<ObjBase> fos = mOb.Httest(hitbox);

            for (int i = 0; i < fos.Count; i++)
            {

                if (fos[i].mType == Emy1.gType || fos[i].mType == Emy2.gType)
                {
                    mOb.Attack1(fos[i]);
                }

            }
            hitbox.SetActive(true);
             

        }
        
        JumpProcess(om);

        if (mbFalldown)
        { fallDown(om); }
        if(mOb.getPos().y <= f1jumping && (mbJumpDown == true||mbFalldown==true))
        {
            Debug.Log("mbfo=tr");
            mbJumpDown = false;
            mbJumpUp = false;
            mbFalldown = false;
            jumping = false;
            this.transform.position = new Vector3(transform.position.x, oldPos.y, transform.position.z);

        }


        if (wait == true)
        {
            Wait();
        }
       

    }
    void Wait()
    {
        waitTime += Time.deltaTime;
        canmove = false;
        Debug.Log("기다리기");
        if (waitTime > 1.0f)
        {
            Debug.Log("나가기~");
            canmove = true;
            wait = false;
            waitTime = 0;
        }
    }
    public void JumpProcess(Om om)
    {
        
        if (mbJumpUp == true)
        {
            Debug.Log("mbJU=true");
            transform.position = new Vector3(transform.position.x, transform.position.y + jumppower, transform.position.z);
            

        }
        if (endjump >= maxJump)
        {
            mbJumpUp = false;
            mbJumpDown = true;
        }
       

        if (mbJumpDown == true)
        {
            doubleJ = false;
            Debug.Log("mbJD=t rue");
            JumpDown(om);
        }
        doubleJump();



    }
    public ObjBase mFloor = null;
    public void fallDown(Om om)
    {
       
        Debug.Log("FallDown(om)");
        transform.position = new Vector3(transform.position.x, transform.position.y - jumppower, transform.position.z);
        

        
        List<ObjBase> fos = om.find2fG(mOb.getFoot().x, mOb.getFoot().y, mOb.getFoot(oldPos).y);
       
        if (fos.Count > 0)
        {
            Vector3 pos = mOb.getPos();
            Debug.Log("최대: " + (fos[0].getPos().x +fos[0].getTest().x));
            Debug.Log("최소: " + (fos[0].getPos().x - fos[0].getTest().x));
            //mFloor = fos[0];
            FloorChange(fos[0]);
            Debug.Log("fos: " + fos[0].getPos().x + fos[0].getTest().x);
            Debug.Log("pos: "+pos.x);
            transform.position = new Vector3(pos.x, fos[0].getPos().y + fos[0].getTest().y , pos.z);
            Debug.Log("T: " + (fos[0].getPos().x + fos[0].getTest().x > pos.x));
            
       }
        


        Debug.Log(fos.Count);
        //for (int i = 0; i < fos. Count; i++)
        //{ 
        //    Debug.Log("성이길바람");
           

        //    if(
        //     transform.position.x - f2G.transform.position.x < f2G.w / 2 &&
        //        transform.position.x - f2G.transform.position.x > -f2G.w / 2)
        //    {

        //        transform.position = new Vector3(transform.position.x, f2G.transform.position.y + f2G.h / 2, transform.position.z);
        //        Debug.Log("성공");
        //        f2jump = true;

        //    }
        //    else
        //    {
        //        Debug.Log("망");
        //        f2jump = false;
        //    }
        //}
        

    }
    public void JumpDown(Om om)
    {
        //if (mbJumpDown == true)
        {
            //Debug.Log("falldown: " + maxJump+" y: "+transform.position.y+" firstJ: "+firstJump);
           
            fallDown(om);
            
          
           
            
         
            
        }
       
        
    }
    public void FloorChange(ObjBase floor)
    {
        mFloor = floor;
        mbJumpDown = false;
        mbJumpUp = false;
        mbFalldown = false;
        jumping = false;
        
    }
    public void doubleJump()
    {
        if (doubleJ)
        {
            mbJumpUp = false;
            Debug.Log("더블점프");
            transform.position = new Vector3(transform.position.x+speed*2*Time.deltaTime, transform.position.y + jumppower, transform.position.z);
        }
        if (endjump >= maxJump)
        {
            Debug.Log("잉");
            doubleJ = false;
            mbJumpDown = true;
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "Ground")
        {

            Debug.Log("점프카운트 복구");
            jumpcount = 2;
            canmove = true;
            attacktime = true;

            ground = false;
            fly = 0;
          
           

        }
        if (collision.gameObject.tag == "2fGround")
        {
            Debug.Log("2층");
            canmove = false;

            jumpcount = 0;
         
          

        }
        if (collision.gameObject.tag == "On2fGround")
        {
            Debug.Log("점프카운트 복구");
            jumpcount = 2;
            canmove = true;
            attacktime = true;

            ground = true;
            fly = 0;
           
            
        }
    }
  
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collie.gameObject.tag == "Line")
        {
            Debug.LogError("나감");

            rigid.drag = 1;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Line")
        {
           
            Debug.Log("줄");
            if (Input.GetAxisRaw("Vertical")>0)
            {
                collie.isTrigger = true;
                vector = Vector3.zero;
                
                vector = Vector3.up;
                canmove = false;
                Debug.Log("줄타기위");
                rigid.drag = 10000;
                transform.position += vector*speed*Time.deltaTime;
            }
            else if(Input.GetAxisRaw("Vertical") < 0){

                collie.isTrigger = true;
                vector = Vector3.zero;
                vector = Vector3.down;
              
                canmove = false;
               
                Debug.Log("줄타기밑");
                rigid.drag = 10000;
                transform.position += vector * speed * Time.deltaTime;

            }
            if ((Input.GetAxisRaw("Horizontal") >=0 || Input.GetAxisRaw("Horizontal") <= 0)&&Input.GetKeyDown(KeyCode.Space))
            {
                rigid.drag = 1;
                canmove = true;
                jumpcount = 2;
                
            }
        }

        if (collision.gameObject.tag == "LineOut")
        {

            rigid.drag = 1;
            collie.isTrigger = false;


        }


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {


        
        if (collision.gameObject.tag == "GOut")
        {
            Debug.Log("나가기~");
            rigid.drag = 1;
            collie.isTrigger = true;
        }
    }




    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (collision.gameObject.tag == "Ground")
    //    {
    //        collie.isTrigger = false;

    //    }

    //}
    private void FixedUpdate()
    {
       
        if (mOb.time > 0.2f)
        {
            
            mOb.HitboxR.SetActive(false);
            mOb.HitboxL.SetActive(false);
                mOb.time = 0;

        }



         
       
        Move();
       




    }
    public void testJump() {
        if (!jump || jumpcount == 0)
        {
          

            return ;

        }
        
        rigid.velocity = Vector2.zero;
        Vector2 jumpvector = new Vector2(0, jumppower);



        if (jumpcount == 1)
        {
            canmove = false;
            
            if (Input.GetAxisRaw("Horizontal") >0||Input.GetAxisRaw("Horizontal")==0)
            {
              
               jumpvector = new Vector2(speed, jumppower);
                
            }
            if (Input.GetAxisRaw("Horizontal")<0)
            {
               
                jumpvector = new Vector2(-speed, jumppower);
               
            }
           
            
            rigid.AddForce(jumpvector, ForceMode2D.Impulse);
           
            

        }
        if (jumpcount == 2 && mOb.righ == "Right")
        {
            Debug.Log("오");
         ;
            attacktime = false;
            rigid.AddForce(jumpvector, ForceMode2D.Impulse);
            
        }
        if(jumpcount==2&&mOb.righ.Equals("Left"))
        {
            Debug.Log("왼");
           
          
            attacktime = false;
            rigid.AddForce(jumpvector, ForceMode2D.Impulse);
        }
        jump = false;
        jumpcount--;
    }
    //IEnumerator Jump()
    //{
    //    if (!jump || jumpcount == 0)
    //    {


    //        yield break;
    //    }
    //    rigid.velocity = Vector2.zero;
    //    Vector2 jumpvector = new Vector2(0, jumppower);



    //    if (jumpcount == 1)
    //    {
    //        canmove = false;
    //        Debug.Log("이단점프!");
    //        jumpvector = new Vector2(speed, jumppower);
    //        jumpcount = 0;
    //        rigid.AddForce(jumpvector, ForceMode2D.Impulse);



    //    }
    //    if (jumpcount == 2)
    //        rigid.AddForce(jumpvector, ForceMode2D.Impulse);


    //    jump = false;
    //    jumpcount--;
    //}

  public  bool mbFalldown = false;
    public void Move()
    {
        vector = Vector3.zero;
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            vector = Vector3.right;
            mOb.righ = "Right";
            //transform.position = new Vector3(transform.position.x + speed*Time.deltaTime, transform.position.y, transform.position.z);

        }
        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            //transform.position = new Vector3(transform.position.x  -speed * Time.deltaTime, transform.position.y, transform.position.z);
            vector = Vector3.left; 
            mOb.righ = "Left";

        }
        Debug.Log(" " + mOb.righ);
        transform.position += vector * speed * Time.deltaTime;

        if (mFloor != null)
        {
            if ((mFloor.getPos().x + mFloor.getTest().x/2) <= transform.position.x ||(mFloor.getPos().x - mFloor.getTest().x/2) >= transform.position.x)
            {
                Debug.Log("ad");
                mbFalldown = true;
            }
        }



    }



    public void lowerJump()
    {

        canmove = false;
        jumpcount = 1;
        collie.isTrigger = true;
        transform.position=new Vector3(transform.position.x, transform.position.y-0.6f, transform.position.z);
       
            
          
        
        
        
    }
    
}