
export class ObjectBase {
    id: string;
    createdBy: string;
    created: Date;
    lastModified: Date;
    lastModifiedBy: string;
    isDeleted: boolean;
}

export class KeyValue {
    id: string;
    value: string;
}

export class WishlistItem extends ObjectBase {

    wishlistId: string;
    learningObjectiveId: string;
    learningObjectiveCode: string;
    learningObjectiveTitle: string;
    numberOfItems: number;
    todo: number;
    deadline: Date;
    itemStatusCount: ItemStatusCount[];
    items: string[];
}

export class DateCount {
    date: Date;
    count: number;
}

export class Domain extends ObjectBase {
    parent: string;
    title: string;
}

export class AccountModel {
    username: string;
    password: string;
}


export class Feedback {
    screeningItemId: string;
    value: string;
}

export class WastedItem {
    itemId: string;
    removedByAuthor: boolean;
    rounds: number;
}

export class VersionedItem extends ObjectBase {
    version: number;
    VersionedLearningObjectiveId: string[];
    itemTypeVersion: ItemType[];
    itemStatusVersion: ItemStatus[];
    titleVersion: string[];
    bodyTextVersion: string[];
    bodyMediaVersion: string[];
    distractorsVersion: Distractor[][];
    keyVersion: string[];
}

export class Item extends ObjectBase {
    description: string;
    code: string;
    title: string;
    itemType: ItemType;
    learningObjectiveId: string;
    wishListId: string;
    itemStatus: ItemStatus;
    version: number;
    bodyText: string;
    //  distractors: Distractor[];
    key: string;
    notes: string;
    attachmentIds: string[]; // Guids
    domainTitle: string;
    simpleChoices: SimpleChoice[];
    wishListTitle: string;
    learningObjectiveTitle: string;
    latestScreeningId: string;
    latestScreeningAuthorId: string;
    deadline: Date;
    uniqueCode: string;
}

export class SimpleChoice {
    title: string;
    isKey: boolean;
}

export class ItemSummary extends ObjectBase {
    code: string;
    title: string;
    bodyText: string;
    itemType: ItemType;
    wishListId: string;
    learningObjectiveId: string;
    itemStatus: ItemStatus;
    version: number;
    key: string;
    deadline: Date;
    learningObjectiveTitle: string;
    domainTitle: string;
    screeners: string;
    screeningCount: number;
    acceptedBy: string;
    author: string;
    latestScreeningId: string;
    latestScreeningAuthorId: string;
    uniqueCode: string;
}

export class Distractor {
    title: string;
}

export enum ScreeningStatus {
    draft = 0,
    final = 1
}


export enum ItemStatus {
    draft = 0,
    readyForReview = 1,
    inReview = 2,
    needsWork = 3,
    accepted = 4,
    rejected = 5,
    deleted = 6,
}

export enum ItemType {
    mc = 0,
    sa = 1,
    hotspot = 2,
    graphicgapmatch = 3
}

export class LearningObjective extends ObjectBase {
    domainId: string;
    code: string;
    title: string;
    domainTitle: string;
    deadline: Date;
    wishlistId: string;
    total: number;
    todo: number;
    createdCount: number;
}

export class Media extends ObjectBase {
    mediaType: string;
    data: number[];
    thumbnail: number[];
}

export class Screening extends ObjectBase {
    itemId: string;
    status: ScreeningStatus;
    basedOnVersion: number;
    nextItemStatus: ItemStatus;
    feedbackList: Feedback[];
}

export class ItemStatusCount {
    itemStatus: ItemStatus;
    count: number;
}

export class UserItemStatusCount extends ItemStatusCount {
    userId: string;
}

export class ScreeningItem extends ObjectBase {
    code: string;
    category: string;
    value: string;
    itemType: ItemType;
}

export class User extends ObjectBase {
    userType?: UserType;
    name: string;
    username: string;
    organisation: string;
    email: string;
    password: string;
    picture: string;
    tokenId: string;
    allowedWishlists: KeyValue[];
    selectedWishlist: KeyValue;
}

export class ScreeningList extends ObjectBase {
    title: string;
    items: ScreeningItem[];
}

export enum UserType {
    constructeur = 1,
    toetsdeskundige = 2,
    restrictedManager = 3,
    manager = 4,
    admin = 5
}

export class UserMetadata {
    idToken: string;
    email: string;
    name: string;
    picture: string;
    userType: UserType;
}

export class VersionedProperty<T> {
    version: number;
    property: T;
    deleted: boolean;
}

export class Wishlist extends ObjectBase {
    screeningsListId: string;
    title: string;
    wishListItems: WishlistItem[];
    disabledItemTypes: ItemType[];
}

export class DomainStats {
    domainId: string;
    domainName: string;
    totalItemCount: number;
    acceptedItemCount: number;
    totalIterationCount: number;
    percentageAccepted: number;
    meanReviewIterations: number;
}

export class ScreeningItemStats {
    screeningItemId: string;
    screeningItemName: string;
    useCount: number;
}

export class UserStats {
    userId: string;
    userName: string;
    picture: string;
    userType: UserType;
    itemsAcceptedCount: number;
    iterationCount: number;
    screeningItemStatsList: ScreeningItemStats[];
    meanReviewIterations: number;
    percentageRejected: number;
}

export class StatsPerWishlist extends ObjectBase {
    itemsAcceptedCount: number;
    itemsInReviewCount: number;
    itemsRejectedCount: number;
    itemsTodoCount: number;
    itemTargetCount: number;
    iterationCount: number;
    percentageAccepted: number;
    percentageMortality: number;
    meanReviewIterations: number;
    statsPerDomain: DomainStats[];
    statsPerUser: UserStats[];
    itemDeadlinesWithCounts: DateCount[];
    itemsAcceptedPerDayCumulative: DateCount[];
    topScreeningItemStats: ScreeningItemStats[];
    screeningRoundsOfWastedItems: WastedItem[];
}

export class Auth0Settings {
    auth0ClientId: string;
    auth0TenantUrl: string;
}
