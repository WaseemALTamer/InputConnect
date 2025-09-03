using InputConnect.UI.Containers.Common;
using InputConnect.Network;
using Avalonia.Threading;
using Avalonia.Controls;
using Avalonia;
using System;
using System.Text.Json;
using InputConnect.SharedData;
using Avalonia.Platform;
using System.Collections.Generic;
using InputConnect.Structures;






namespace InputConnect.UI.Containers
{
    class MointorsGraph : Border
    {

        private Canvas? Master;
        
        private Canvas? _MainCanvas;
        public Canvas? MainCanvas{
            get { return _MainCanvas; }
            set { _MainCanvas = value; }
        }

        private Graph? _MainGraph;
        public Graph? MainGraph{
            get { return _MainGraph; }
            set { _MainGraph = value; }
        }

        public MointorsGraph(Canvas? master = null) {

            Master = master;
            CornerRadius = new CornerRadius(Setting.Config.CornerRadius);
            ClipToBounds = true;


            MainCanvas = new Canvas();
            MainCanvas.ClipToBounds = true;


            MainGraph = new Graph(600, 300); // the size of the graph is static
            MainCanvas.Children.Add(MainGraph);

            
            



            //PointerPressed += OnPointerPressed;

            OnSizeChage();

            if (Master != null) {
                Master.PropertyChanged += OnSizeChage;
            }


            MessageManager.OnAccept += _ => Update();
            Connections.Manager.OnConnectedConnectionAdded += Update;
            Connections.Manager.OnConnectionClosed += Update;
            Events.TargetDeviceConnectionChanged += Update;

            UpdateGraph();
            Child = MainCanvas;
        }


        public void OnPointerPressed(object? sender, object? e) {
            UpdateGraph();
        }

        public void Update(){
            Dispatcher.UIThread.Post(() => UpdateGraph());  
        }

        public async void UpdateGraph() {
            
            if (MainGraph == null) return;


            MainGraph.ClearGraphObjects();
            // add my own physical screens

            int mointorNumer = 1;
            int MaxPosX = 0; // this will control 

            if (SharedData.Device.Screens != null){
                foreach (var screen in SharedData.Device.Screens){
                    var mointor = new Monitor(screen.Bounds.X,
                                              screen.Bounds.Y,
                                              screen.Bounds.Width,
                                              screen.Bounds.Height,
                                              mointorNumer,
                                              MainGraph);

                    mointor.ScreenBounds = null; // this doesnt use the screen bounds as its only going to be locked
                                                 // its the physical mointors on your computer  if  you  thinking of 
                                                 // changing them use the system setting to change there postion
                    mointor.SetLock(true);
                    
                    mointorNumer += 1;
                    MainGraph.AddGraphObject(mointor);


                    if (MaxPosX < screen.Bounds.X + screen.Bounds.Width)
                        MaxPosX = screen.Bounds.X + screen.Bounds.Width;
                }
            }

            // add other connected devices screens
            if (Connections.Devices.ConnectionList != null){
                
                foreach (var device in Connections.Devices.ConnectionList){
                    if (device.Screens == null) {

                        if (device.State != Connections.Constants.StateConnected) 
                            continue;

                        // we need to request the data of the other device if we 
                        // dont already have it
                        await Connections.Manager.RequestInitialData(device);
                        if (device.Screens == null) {
                            continue;
                        }
                    }


                    if (device.VirtualScreens != null) {
                        foreach (var screen in device.VirtualScreens){

                            var mointor = new Monitor(screen.X,
                                                      screen.Y,
                                                      screen.Width,
                                                      screen.Height,
                                                      mointorNumer,
                                                      MainGraph);
                            mointor.Connection = device;

                            mointor.ScreenBounds = screen;

                            mointor.SetLock(false);

                            mointorNumer += 1;
                            MainGraph.AddGraphObject(mointor);

                            if (screen.X + screen.Width > MaxPosX)
                                MaxPosX = screen.X + screen.Width;
                        }

                        continue;
                    }




                    if (device.VirtualScreens == null){
                        device.VirtualScreens = new List<Bounds>();

                        if (device.Screens != null){
                            foreach (var s in device.Screens)
                            {
                                device.VirtualScreens.Add(new Bounds
                                {
                                    X = MaxPosX,
                                    Y = s.Y,
                                    Width = s.Width,
                                    Height = s.Height
                                });


                                MaxPosX = s.X + s.Width;

                            }
                        }

                        foreach (var screen in device.VirtualScreens){

                            var mointor = new Monitor(screen.X,
                                                      screen.Y,
                                                      screen.Width,
                                                      screen.Height,
                                                      mointorNumer,
                                                      MainGraph);

                            mointor.ScreenBounds = screen;

                            mointor.Connection = device;

                            mointor.SetLock(false);

                            mointorNumer += 1;
                            MainGraph.AddGraphObject(mointor);


                            if (screen.X + screen.Width > MaxPosX)
                                MaxPosX = screen.X + screen.Width;
                        }
                    }


                }
            }


        }



        public void OnSizeChage(object? sender = null, object? e = null) {

            if (Master == null) return;

            Width = Master.Width - 425;
            Height = Master.Height - 175;
            //Height = 390; // for static height approch i didnt like it

            if (MainCanvas != null){
                MainCanvas.Width = Width;
                MainCanvas.Height = Height;

                if (MainGraph != null) {
                    MainGraph.Width = Width;
                    MainGraph.Height = Height;

                    if (MainGraph.MasterCanvas != null) {
                        MainGraph.MasterCanvas.Width = Width;
                        MainGraph.MasterCanvas.Height = Height;
                        MainGraph.Update(); // trigger the scroll triger to keep  the grid in check
                                            // this is not a good way to solve the bug but it works
                                            // for now, change it later
                    }



                    Canvas.SetLeft(MainGraph, (MainCanvas.Width - MainGraph.Width)/2);
                    Canvas.SetTop(MainGraph, (MainCanvas.Height - MainGraph.Height)/2);
                }
            }





        }

    }
}
