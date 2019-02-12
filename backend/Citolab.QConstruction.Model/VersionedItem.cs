using System;
using System.Linq;
using Newtonsoft.Json;

namespace Citolab.QConstruction.Model
{
    /// <summary>
    ///     Item history
    ///     Contains for each property that should be versioned, two properties: One with the same name as in the item with can
    ///     be set using reflection
    ///     and one property that contains the  changed fields.
    /// </summary>
    public class VersionedItem : Citolab.Repository.Model
    {
        /// <summary>
        ///     ctor
        /// </summary>
        public VersionedItem()
        {
            VersionedItemStatus = new VersionedProperty<ItemStatus>
            {
                new PropertyVersion<ItemStatus>(0, ItemStatus.Draft)
            };
        }

        /// <summary>
        ///     Version number.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        ///     Identifier of the learning objective to which this item belongs.
        /// </summary>
        public VersionedProperty<Guid> VersionedLearningObjectiveId { get; set; }

        /// <summary>
        ///     The item type.
        /// </summary>
        public VersionedProperty<ItemType> VersionedItemType { get; set; }

        /// <summary>
        ///     Status of the item.
        /// </summary>
        public VersionedProperty<ItemStatus> VersionedItemStatus { get; set; }

        /// <summary>
        ///     Title of the item.
        /// </summary>
        public VersionedProperty<string> VersionedTitle { get; set; }

        /// <summary>
        ///     Body text of the item.
        /// </summary>
        public VersionedProperty<string> VersionedBodyText { get; set; }

        /// <summary>
        ///     Options for this item, in case of item type MC/MR.
        /// </summary>
        public VersionedProperty<SimpleChoice[]> VersionedSimpleChoices { get; set; }

        /// <summary>
        ///     Key for the item.
        /// </summary>
        public VersionedProperty<string> VersionedKey { get; set; }

        #region Property to get the lastest version and set a new version

        /// <summary>
        ///     Set Changed Learning Objective
        /// </summary>
        [JsonIgnore]
        public Guid LearningObjectiveId
        {
            get { return VersionedLearningObjectiveId.Latest(); }
            set
            {
                if (value == LearningObjectiveId) return;
                if (VersionedLearningObjectiveId == null) VersionedLearningObjectiveId = new VersionedProperty<Guid>();
                VersionedLearningObjectiveId.Add(new PropertyVersion<Guid>(Version, value));
            }
        }

        /// <summary>
        ///     The item type
        /// </summary>
        [JsonIgnore]
        public ItemType ItemType
        {
            get { return VersionedItemType.Latest(); }
            set
            {
                if (value != ItemType) return;
                if (VersionedItemType == null) VersionedItemType = new VersionedProperty<ItemType>();
                {
                    VersionedItemType.Add(new PropertyVersion<ItemType>(Version, value));
                }
            }
        }

        /// <summary>
        ///     Item status
        /// </summary>
        [JsonIgnore]
        public ItemStatus ItemStatus
        {
            get { return VersionedItemStatus.Latest(); }
            set
            {
                if (value == ItemStatus) return;
                if (VersionedItemStatus == null) VersionedItemStatus = new VersionedProperty<ItemStatus>();
                VersionedItemStatus.Add(new PropertyVersion<ItemStatus>(Version, value));
            }
        }

        /// <summary>
        ///     Title
        /// </summary>
        [JsonIgnore]
        public string Title
        {
            get { return VersionedTitle.Latest(); }
            set
            {
                if (value == Title) return;
                if (VersionedTitle == null) VersionedTitle = new VersionedProperty<string>();
                VersionedTitle.Add(new PropertyVersion<string>(Version, value));
            }
        }

        /// <summary>
        ///     BodyText
        /// </summary>
        [JsonIgnore]
        public string BodyText
        {
            get { return VersionedBodyText.Latest(); }
            set
            {
                if (value == BodyText) return;
                if (VersionedBodyText == null) VersionedBodyText = new VersionedProperty<string>();
                VersionedBodyText.Add(new PropertyVersion<string>(Version, value));
            }
        }

        /// <summary>
        ///     Options for this item, in case of item type MC.
        /// </summary>
        [JsonIgnore]
        public SimpleChoice[] Distractors
        {
            get => VersionedSimpleChoices.Latest();
            set
            {
                if (Distractors != null && value.SequenceEqual(Distractors)) return;
                if (VersionedSimpleChoices == null) VersionedSimpleChoices = new VersionedProperty<SimpleChoice[]>();
                VersionedSimpleChoices.Add(new PropertyVersion<SimpleChoice[]>(Version, value));
            }
        }

        /// <summary>
        ///     Key for the item.
        /// </summary>
        [JsonIgnore]
        public string Key
        {
            get => VersionedKey.Latest();
            set
            {
                if (value == Key) return;
                if (VersionedKey == null) VersionedKey = new VersionedProperty<string>();
                VersionedKey.Add(new PropertyVersion<string>(Version, value));
            }
        }

        #endregion
    }
}