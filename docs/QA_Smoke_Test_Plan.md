# QA Smoke Test Plan: Chrono-Core (Phase 1)

## 1. Test Scope
Covers core movement and rewind mechanics for the Phase 1 grey-box prototype.
- **Included:** Player movement, Dash cooldown/i-frames, Rewind buffer, Position restoration, Chrono Energy, Irreversible events.
- - **Excluded:** Combat AI, Level streaming, UI menus.
 
  - ## 2. Test Environment
  - - **Unity Version:** Unity 6 LTS (URP)
    - - **Framework:** Unity Test Framework (UTF) - PlayMode & EditMode
      - - **CI/CD:** GameCI + GitHub Actions
       
        - ## 3. Smoke Test Cases
       
        - **TC-01: Basic Player Movement**
        - - Given player at (0,0). When "Move Right" for 1s. Then position X > 0.
         
          - **TC-02: Dash Cooldown and I-Frames**
          - - Given Idle. When Dash. Then isInvincible=true (0.2s), dashCooldown active (0.8s).
           
            - **TC-03: Rewind Buffer Integrity**
            - - Given 60fps. When active for 5s. Then buffer has 300 entries.
             
              - **TC-04: Position Restoration Accuracy**
              - - Given moved from (0,0) to (10,0). When Rewind for 1s. Then position approx (5,0) (+/- 0.05).
               
                - **TC-05: Chrono Energy Depletion**
                - - Given full energy. When Rewind for 3s. Then energy meter depletes proportionally.
                 
                  - **TC-06: Rewind Hard-Stop at Room Boundary**
                  - - Given Room Lock event. When Rewind. Then stop at room entry point.
                   
                    - **TC-07: Irreversible Event Persistence**
                    - - Given Door Opened. When Rewind. Then Door remains open.
                     
                      - **TC-18: Rewind during Dash**
                      - - Given Dashing. When Rewind. Then Dash canceled, position reverts correctly.
                       
                        - ## 4. Pass/Fail Criteria
                        - - Pass: All TC-01 to TC-08 pass, build compiles, no console errors.
                          - - Fail: Any test failure or build error.
                           
                            - ## 5. Known Failing Tests
                            - - TC-06 & TC-07: Blocked on RoomManager/EventManager architecture.
                             
                              - ## 6. Regression Policy
                              - - Failures block PR merge. Senior QA must sign off on changes.
                                - 
