using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Resources;
using UnityEngine;

namespace Resources
{
    
    [ExecuteInEditMode]
    public class RockGroup : MonoBehaviour, IResourceSite
    {
        public int MaxAmount
        {
            get => rocks.Sum(rock => rock.MaxAmount);
            set => throw new InvalidOperationException();
        }

        public int Amount
        {
            get => rocks.Sum(rock => rock.Amount);
            set
            {
                var val = Math.Max(Mathf.Min(value, MaxAmount), 0);
                foreach (var rock in rocks)
                {
                    rock.Amount = Mathf.Min(val, rock.MaxAmount);
                    val -= rock.Amount;
                }
            }
        }

        public int AM = 100;

        public float Depletion => 1 - ((float) Amount / MaxAmount);
        public string ResourceName { get => "Stone"; }
        public int ResourceId { get => 1; }

        public Rock[] rocks;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Amount = AM;
        }
    }
    
}
