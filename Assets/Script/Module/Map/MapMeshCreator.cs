using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameFramework;
using GameFramework.utils;
using UnityEngine.EventSystems;

namespace Assets.Script.Module.Map
{
    /// <summary>
    /// 自定义地图网咯创建器
    /// </summary>
    public static class MapMeshCreator
    {
        public struct BuildMapConfig
        {
            public float width;
            public float length;
            public float down;
            public float up;
            public int segmentX;
            public int segmentY;
        }


        static MapMeshCreator()
        {
            ArrayPool<Texture2D> pool = ArrayPool<Texture2D>.Shared;

            var buffer = pool.Rent(1);//预先申请1024个对象的内存块,返回该内存区的所有者

            //Array.Clear(buffer,0,buffer.Length);
            //pool.Return(buffer);//释放这一批
        }

        private static Queue<Action> FrameTasks = new Queue<Action>();
        private static void Update()
        {
            if (FrameTasks.Count > 0)
            {
                FrameTasks.Dequeue().Invoke();
            }
        }




        /// <summary>
        /// 根据一张图片异步创建一个地形游戏物体
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public async static Task<Transform> CreateMapObject(Texture2D texture, BuildMapConfig buildCfg)
        {
            FrameTasks.Clear();
            float width = buildCfg.width, length = buildCfg.length, down = buildCfg.down, up = buildCfg.up;
            float heightDistance = (up - down);//高度距离
            int segmentX = buildCfg.segmentX, segmentY = buildCfg.segmentY;//网格长宽段数

            GameObject map = new GameObject("MapObject");//地图物体


            map.layer = LayerMask.GetMask(EMapLayer.MapGround);
            float textureWidth = texture.width;
            float textureHeight = texture.height;

            float w = width / segmentX;//单位像素宽
            float h = length / segmentY;//单位像素高

            int index = 0;
            //UV信息
            Vector2[] uv = new Vector2[segmentX * segmentY];
            FrameTasks.Enqueue(() =>
            {
                float u = 1.0f / segmentX;
                float v = 1.0f / segmentY;

                for (int i = 0; i < segmentY; i++)
                {
                    for (int j = 0; j < segmentX; j++)
                    {
                        uv[index++] = new Vector2(j * u, i * v);
                    }
                }
            });



            //顶点
            Vector3[] vertives = new Vector3[segmentX * segmentY];
            FrameTasks.Enqueue(() =>
            {
                index = 0;
                for (int i = 0; i < segmentY; i++)
                {
                    for (int j = 0; j < segmentX; j++)
                    {
                        Vector2 pointuv = uv[index];
                        Color c = texture.GetPixel((int)Math.Floor(textureWidth * pointuv.x), (int)Math.Floor(textureHeight * pointuv.y));
                        float height = (c.grayscale - 0.2f) * heightDistance;//灰度转为高度值
                        vertives[index] = new Vector3(j * w, height, i * h);
                        index++;
                    }
                }

            });
            //顶点索引
            int[] triangles = new int[(segmentX - 1) * (segmentY - 1) * 6];
            FrameTasks.Enqueue(() =>
            {
                index = 0;
                for (int i = 0; i < segmentY - 1; i++)
                {
                    for (int j = 0; j < segmentX - 1; j++)
                    {
                        /*
                         6,7,8
                         3,4,5
                         0,1,2,
                            [j,i]
                         */

                        int self = j + i * segmentX;
                        int upIndex = j + (i + 1) * segmentX;
                        int uprightIndex = upIndex + 1;
                        int right = self + 1;
                        triangles[index] = self;
                        triangles[index + 1] = upIndex;
                        triangles[index + 2] = uprightIndex;
                        triangles[index + 3] = self;
                        triangles[index + 4] = uprightIndex;
                        triangles[index + 5] = right;
                        index += 6;
                    }
                }

            });

            //创建网格
            FrameTasks.Enqueue(() =>
            {
                Mesh mesh = map.AddComponent<MeshFilter>().mesh;
                mesh.Clear();
                mesh.vertices = vertives;
                mesh.uv = uv;
                mesh.triangles = triangles;
                mesh.RecalculateBounds();

                MeshRenderer renderer = map.AddComponent<MeshRenderer>();
                renderer.material = Resources.Load<Material>("SnowMaterial");

                //创建碰撞网格组件
                MeshCollider coll = map.AddComponent<MeshCollider>();
                coll.sharedMesh.vertices = vertives;
                coll.sharedMesh.uv = uv;
                coll.sharedMesh.triangles = triangles;
                coll.sharedMesh.RecalculateBounds();

            });

            //草地
            GameObject grass = new GameObject("Grass");//草皮
            var grassMf = grass.AddComponent<MeshFilter>();
            var grassMh = grass.AddComponent<MeshRenderer>();
            grassMh.material = Resources.Load<Material>("GrassMaterial");
            var grassMesh = new Mesh();
            grassMf.mesh = grassMesh;
            List<Vector3> grassPoints = new List<Vector3>();
            grass.transform.SetParent(map.transform);

            float gw = width / textureWidth;//单位像素宽
            float gh = length / textureHeight;//单位像素高
            int taskLength = 0;
            List<(int, int)> IJArr = new List<(int, int)>();
            for (int i = 0; i < textureHeight; i += 3)
            {
                for (int j = 0; j < textureWidth; j += 3)
                {
                    IJArr.Add((i, j));
                    if (IJArr.Count > 50 || (i == textureHeight - 1 && j == textureWidth - 1)) //每50个像素点作为一个构建任务
                    {
                        var arr = IJArr.ToArray();
                        FrameTasks.Enqueue(() =>
                        {
                            for (int n = 0; n < arr.Length; n++)
                            {
                                int x = arr[n].Item2;
                                int y = arr[n].Item1;
                                Color c = texture.GetPixel(x, y);
                                if (c.r<0.5f &&  c.b<0.5f && c.g > 0.5f) //绿色值大于0.5
                                {
                                    if (UnityEngine.Random.Range(0f, 1f) <= c.g) //绿色值越大，概率越高
                                    {
                                        //float height = (c.grayscale - 0.2f) * heightDistance;//灰度转为高度值
                              


                                        int grassNum = UnityEngine.Random.Range(3, 20);
                                        while (grassNum > 0)
                                        {
                                            --grassNum;
                                            float rdX = x + UnityEngine.Random.Range(-1.0f, 1.0f);
                                            float rdY = y + UnityEngine.Random.Range(-1.0f, 1.0f);
                                            //c = texture.GetPixel(rdX, rdY);
                                            //float height = (c.grayscale - 0.2f) * heightDistance;//灰度转为高度值
                                            //grassPoints.Add(new Vector3(rdX * gw, height, rdY * gh));
                                            if (Physics.Raycast(new Ray() { origin = new Vector3(rdX, heightDistance + 1, rdY), direction = Vector3.down }, out var hit))
                                            {
                                                grassPoints.Add(hit.point);

                                            }

                                        }


                                    }
                                }

                            }
                        });
                        IJArr.Clear();
                    }


                }
            }

            //草地点
            FrameTasks.Enqueue(() =>
            {
                grassMesh.vertices = grassPoints.ToArray();
                int[] indexs = grassPoints.Select(p => grassPoints.IndexOf(p)).ToArray();
                grassMesh.SetIndices(indexs, MeshTopology.Points, 0);
            });


            bool finishFlag = false;
            FrameTasks.Enqueue(() =>
            {
                finishFlag = true;
            });

            TimeUtil.AddFrame(Update);

            while (!finishFlag)
            {
                await Task.Run(() =>
                {
                    Debug.Log("wait mapMesh bnilding...");
                });
            }
            TimeUtil.RemoveFrame(Update);

            return map.transform;

        }
    }
}
