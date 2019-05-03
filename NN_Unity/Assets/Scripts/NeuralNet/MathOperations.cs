using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Andr3wDown
{
    namespace Math
    {
        public static class MathOperations
        {
            #region Maths
            //range of 0f - 1f
            public static float NormalizeFloat(float value, float min, float max)
            {
                float normalized = (value - min) / (max - min);
                return normalized;
            }
            //opposite
            public static float DenormalizeFloat(float normalizedValue, float min, float max)
            {
                float denormalized = (normalizedValue * (max - min) + min);
                return denormalized;
            }
            //bring float gradually to a value
            public static float BringToValue(float value, float targetValue, float timeDelta, float ratio)
            {
                if (value < targetValue)
                {
                    value += timeDelta * ratio;
                    if (value > targetValue)
                    {
                        value = targetValue;
                    }
                }
                else if (value > targetValue)
                {
                    value -= timeDelta * ratio;
                    if (value < targetValue)
                    {
                        value = targetValue;
                    }
                }

                return value;
            }
            //clamp values
            public static float Clamp(float value, float min, float max)
            {
                if (value < min)
                {
                    value = min;
                }
                else if (value > max)
                {
                    value = max;
                }
                return value;
            }
            public static int Clamp(int value, int min, int max)
            {
                if (value < min)
                {
                    value = min;
                }
                else if (value > max)
                {
                    value = max;
                }
                return value;
            }
            public static double Clamp(double value, double min, double max)
            {
                if (value < min)
                {
                    value = min;
                }
                else if (value > max)
                {
                    value = max;
                }
                return value;
            }
            //abs
            public static float Abs(float value)
            {
                if (value < 0)
                {
                    value = -value;
                }
                return value;
            }
            //a lock a number to range so that it loops to the smallest after reaching biggest and vise versa
            public static float RepeatingRange(float value, float min, float max)
            {
                if (value > max)
                {
                    value = 0 + (value - max);
                }
                else if (value < min)
                {
                    value = max - value;
                }
                return value;
            }

            public static float[] GenerateRandomNumbers(int amount, float min, float max)
            {
                float[] numbers = new float[amount];
                for (int i = 0; i < numbers.Length; i++)
                {
                    numbers[i] = UnityEngine.Random.Range(min, max);
                }
                return numbers;
            }
            #endregion
            #region Activation Functions
            //NeuralNet Activation functions

            //Rectified Linear Unit - ReLU
            public static float ReLU(float valueToConvert)
            {
                if(valueToConvert <= 0)
                {
                    valueToConvert = 0;
                }
                return valueToConvert;
            }
            public static double ReLU(double valueToConvert)
            {
                if (valueToConvert <= 0)
                {
                    valueToConvert = 0;
                }
                return valueToConvert;
            }
            //ReLU derivative
            public static float ReLUDerivative(float valueToDerive)
            {
                if(valueToDerive <= 0)
                {
                    valueToDerive = 0;
                }
                else
                {
                    valueToDerive = 1;
                }
                return valueToDerive;
            }
            public static double ReLUDerivative(double valueToDerive)
            {
                if (valueToDerive <= 0)
                {
                    valueToDerive = 0;
                }
                else
                {
                    valueToDerive = 1;
                }
                return valueToDerive;
            }
            //leaky ReLU
            public static float LeakyReLU(float valueToConvert)
            {
                if (valueToConvert <= 0)
                {
                    valueToConvert = valueToConvert * 0.01f;
                }
                return valueToConvert;
            }
            public static double LeakyReLU(double valueToConvert)
            {
                if (valueToConvert <= 0)
                {
                    valueToConvert = valueToConvert * 0.01f;
                }
                return valueToConvert;
            }
            //leaky ReLU Derivative
            public static float LeakyReLUDerivative(float valueToDerive)
            {           
                if (valueToDerive <= 0)
                {
                    valueToDerive = 0.1f;
                }
                else
                {
                    valueToDerive = 1;
                }
                return valueToDerive;            
            }
            public static double LeakyReLUDerivative(double valueToDerive)
            {
                if (valueToDerive <= 0)
                {
                    valueToDerive = 0.1;
                }
                else
                {
                    valueToDerive = 1;
                }
                return valueToDerive;
            }
            //SoftMax
            public static float Softmax(float valueToConvert)
            {
                return Mathf.Log(1 + Mathf.Exp(valueToConvert));
            }
            public static double Softmax(double valueToConvert)
            {
                return System.Math.Log(1 + System.Math.Exp(valueToConvert));
            }
            //SoftMax Derivative
            public static float SoftmaxDerivative(float valueToDerive)
            {
                return 1 / (1 + Mathf.Exp(-valueToDerive));
            }
            public static double SoftmaxDerivative(double valueToDerive)
            {
                return 1 / (1 + System.Math.Exp(-valueToDerive));
            }
            //TanH
            public static float TanH(float valueToConvert)
            {
				return (float)System.Math.Tanh(valueToConvert);//(Mathf.Exp(valueToConvert) - Mathf.Exp(-valueToConvert)) / (Mathf.Exp(valueToConvert) + Mathf.Exp(-valueToConvert));
            }
            public static double TanH(double valueToConvert)
            {
				return System.Math.Tanh(valueToConvert);//(System.Math.Exp(valueToConvert) - System.Math.Exp(-valueToConvert)) / (System.Math.Exp(valueToConvert) + System.Math.Exp(-valueToConvert));
            }
            //Derivative of TanH
            public static float TanHDerivative(float valueToDerive)
            {
				return 1f - valueToDerive * valueToDerive;//TanH(valueToDerive) * TanH(valueToDerive);
            }
            public static double TanHDerivative(double valueToDerive)
            {
				return 1.0 - valueToDerive * valueToDerive;//TanH(valueToDerive) * TanH(valueToDerive);
            }
            //Sigmoid
            public static float Sigmoid(float valueToConvert)
            {
                return 1f / (1f + Mathf.Exp(-valueToConvert));
            } 
            public static double Sigmoid(double valueToConvert)
            {
                return 1.0 / (1.0 + System.Math.Exp(-valueToConvert));
            }
            //sigmoid derivative
            public static float SigmoidDerivative(float valueToDerive)
            {
				return valueToDerive * (1f - valueToDerive);//Sigmoid(valueToDerive) - (1f - Sigmoid(valueToDerive));
            }
            public static double SigmoidDerivative(double valueToDerive)
            {
				return valueToDerive * (1.0 - valueToDerive);//Sigmoid(valueToDerive) - (1.0 - Sigmoid(valueToDerive));
            }
            #endregion
            #region UnityOperations
            //easy way to limit an objets rigidbodyvelocity
            //index is used if there's a value you don't want to limit on the velocity vector like the y axis for example (0 = x, 1 = y, 2 = z)
            public static Vector3 LimitVelocity(Vector3 velocityVector, float maxSpeed, float currentSpeed, int index)
            {
                if (currentSpeed > maxSpeed)
                {
                    switch (index)
                    {
                        case 0:
                            velocityVector = new Vector3(velocityVector.normalized.x, velocityVector.y * maxSpeed, velocityVector.normalized.z * maxSpeed);
                            break;

                        case 1:
                            velocityVector = new Vector3(velocityVector.normalized.x * maxSpeed, velocityVector.y, velocityVector.normalized.z * maxSpeed);
                            break;

                        case 2:
                            velocityVector = new Vector3(velocityVector.normalized.x * maxSpeed, velocityVector.y * maxSpeed, velocityVector.normalized.z);
                            break;

                        case 3:
                            velocityVector = new Vector3(velocityVector.normalized.x * maxSpeed, velocityVector.y * maxSpeed, velocityVector.normalized.z * maxSpeed);
                            break;
                    }
                }
                return velocityVector;
            }
            //transform a vector into another one gradually over time (same as the float one(can be pretty dubious don't recommend using for important stuff))
            public static Vector3 BringVectorToValue(Vector3 vect, float target, float timeDelta, float ratio)
            {
                vect.x = BringToValue(vect.x, target, timeDelta, ratio);
                vect.y = BringToValue(vect.y, target, timeDelta, ratio);
                vect.z = BringToValue(vect.z, target, timeDelta, ratio);
                return vect;
            }
            public static Vector3 BringVectorToValue(Vector3 vect, float target, float timeDelta, float ratio, int index)
            {
                if (index != 0)
                    vect.x = BringToValue(vect.x, target, timeDelta, ratio);
                if (index != 1)
                    vect.y = BringToValue(vect.y, target, timeDelta, ratio);
                if (index != 2)
                    vect.z = BringToValue(vect.z, target, timeDelta, ratio);
                return vect;
            }
            public static Vector3 BringVectorToValue(Vector3 vect, Vector3 target, float timeDelta, float ratio)
            {
                vect.x = BringToValue(vect.x, target.x, timeDelta, ratio);
                vect.y = BringToValue(vect.y, target.y, timeDelta, ratio);
                vect.z = BringToValue(vect.z, target.z, timeDelta, ratio);
                return vect;
            }
            public static Vector3 BringVectorToValue(Vector3 vect, Vector3 target, float timeDelta, float ratio, int index)
            {
                if (index != 0)
                    vect.x = BringToValue(vect.x, target.x, timeDelta, ratio);
                if (index != 1)
                    vect.y = BringToValue(vect.y, target.y, timeDelta, ratio);
                if (index != 2)
                    vect.z = BringToValue(vect.z, target.z, timeDelta, ratio);
                return vect;
            }
            //find the closest target from a list or array of many
            public static Transform FindClosestTarget(List<Transform> objList, Transform searcher)
            {
                float distance = float.MaxValue;
                Transform target = null;

                for (int i = 0; i < objList.Count; i++)
                {
                    if (Vector3.Distance(searcher.position, objList[i].position) < distance)
                    {
                        distance = Vector3.Distance(searcher.position, objList[i].position);
                        target = objList[i];
                    }
                }

                return target;
            }
            public static Transform FindClosestTarget(List<Collider> objList, Transform searcher)
            {
                float distance = float.MaxValue;
                Transform target = null;

                for (int i = 0; i < objList.Count; i++)
                {
                    if (Vector3.Distance(searcher.position, objList[i].transform.position) < distance)
                    {
                        distance = Vector3.Distance(searcher.position, objList[i].transform.position);
                        target = objList[i].transform;
                    }
                }

                return target;
            }
            public static Transform FindClosestTarget(Transform[] objArr, Transform searcher)
            {
                float distance = float.MaxValue;
                Transform target = null;

                for (int i = 0; i < objArr.Length; i++)
                {
                    if (Vector3.Distance(searcher.position, objArr[i].position) < distance)
                    {
                        distance = Vector3.Distance(searcher.position, objArr[i].position);
                        target = objArr[i];
                    }
                }

                return target;
            }
            public static Transform FindClosestTarget(Collider[] objArr, Transform searcher)
            {
                float distance = float.MaxValue;
                Transform target = null;

                for (int i = 0; i < objArr.Length; i++)
                {
                    if (Vector3.Distance(searcher.position, objArr[i].transform.position) < distance)
                    {
                        distance = Vector3.Distance(searcher.position, objArr[i].transform.position);
                        target = objArr[i].transform;
                    }
                }

                return target;
            }

            public static Quaternion LookAt2D(Vector3 target, Vector3 position, float angleOffset)
            {
                Vector3 diff = target - position;
                diff.Normalize();

                float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                return Quaternion.Euler(0f, 0f, rot_z + angleOffset);
            }
			public static Vector2 RadianToVector2(float radian)
			{
				return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
			}
			public static Vector2 RadianToVector2(float radian, float length)
			{
				return RadianToVector2(radian) * length;
			}
			public static Vector2 AngleToVector(float angle)
			{
				return RadianToVector2(angle * Mathf.Deg2Rad);
			}
			public static Vector2 AngleToVector(float angle, float length)
			{
				return RadianToVector2(angle * Mathf.Deg2Rad) * length;
			}
			public static float VectorToAngle(Vector2 diff)
			{
				diff.Normalize();
				float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
				return rot_z;
			}
			#endregion
		}
		public static class Gaussian
		{
			private static System.Random gen = new System.Random();

			public static float GetRandomGaussian()
			{
				return GetRandomGaussian(-1.0f, 1.0f);
			}

			public static void GetRandomGaussian(float mean, float stddev, out float val1, out float val2)
			{
				float u, v, s, t;
				do
				{
					u = 2 * (float)gen.NextDouble() - 1;
					v = 2 * (float)gen.NextDouble() - 1;

				} while (u * u + v * v > 1 || (u == 0 && v == 0));

				s = u * u + v * v;
				t = Mathf.Sqrt((2.0f * Mathf.Log(s)) / s);

				val1 = stddev * u * t + mean;
				val2 = stddev * v * t + mean;
			}

			public static float GetRandomGaussian(float mean, float stddev)
			{
				float rval1, rval2;
				GetRandomGaussian(mean, stddev, out rval1, out rval2);
				return rval1;
			}
		}
	}
}