export interface Comment {
    id: string;
    pinId: string;
    text: string;
    createdAt: string;
}

export interface Pin {
    id: string;
    projectId: string;
    content: string;
    x: number;
    y: number;
    createdAt: string;
    comments: Comment[];
}

export interface Project {
    id: string;
    title: string;
    imageUrl: string;
    width: number;
    height: number;
    pins: Pin[];
}