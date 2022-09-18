#pragma once
#include<iostream>
#define MAXVALUE 101
using namespace std;

template<class T> class myQueue
{
public:
    int size; // queue max size.
    int rear; // queue back index.
    int front; // queue front index.
    T* values; // queue using array.

    myQueue()
    {
        front = -1;
        rear = -1;
        size = MAXVALUE;
        values = new T[size];
    }

    ~myQueue() {
        delete[] values;
    }

    // queue가 다 찼다면 true return , else false return.

    bool isfull()
    {
        if (size != 0)
            if ((rear + 1) % size == front)
            {
                return true;
            }
        return false;
    }

    // 비어있다면 true, else false.

    bool empty() {
        return (front == -1) ? true : false;
    }

    // 데이터 queue에 입력.
    void push(T value)
    {
        if (empty()) {
            values[++rear] = value;
            front++;
        }
        else if (isfull()) {
            cout << "queue is full!!" << endl;
        }
        else {
            rear = (rear + 1) % size;
            values[rear] = value;
        }
    }

    // 맨 앞의 값 꺼내기.(선입선출)
    void pop() {
        if (empty()) cout << "queue is empty! pop doesn't work." << endl;
        else if (front == rear && front != -1) {
            front = -1;
            rear = -1;
        }
        else {
            front = (front + 1) % size;
        }
    }

    // 가장 최근에 들어온 데이터 반환
    T queue_back() {
        if (empty()) {
            return nullptr;
        }
        return values[rear];
    }

    // 가장 먼저 들어온 데이터 반환
    T queue_front() {
        if (empty()) {
            return nullptr;
        }
        return values[front];
    }

    // 현재 queue의 데이터 갯수
    int queue_size() {
        int i = front;
        int cnt = 1;
        if (empty()) return -1;
        else if (isfull()) return size;

        while (i != rear) {
            i = (i + 1) % size;
            cnt++;
        }
        return cnt;
    }
};