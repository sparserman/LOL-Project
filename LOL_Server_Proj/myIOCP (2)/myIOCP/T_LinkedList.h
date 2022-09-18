#include <iostream>

template<typename Type>
class Node
{
public:
    Type VALUE;
    Node<Type>* Link;
};

template<typename Type>
class myList
{
private:
    Node<Type>* header;
public:
    myList();
    ~myList();
    void Add_First(Type Item);
    void Add(int Position, Type Item);
    Type Delete(int Position);
    void Replace(int Position, Type Item);
    Type Find(Type Item);
    void Get_Entry(int Position);
    void Get_Length();
    bool Is_Empty();
    void Printing();
};

template<typename Type>
myList<Type>::myList()
{
    this->header = NULL;
}

template<typename Type>
myList<Type>::~myList()
{

}

template<typename Type>
void myList<Type>::Add_First(Type Item)
{
    Node<Type>* New = new Node<Type>;
    if (this->header == NULL)
    {
        this->header = New;
        New->VALUE = Item;
        New->Link = NULL;
        return;
    }
    if (this->header != NULL)
    {
        New->Link = this->header;
        this->header = New;
        New->VALUE = Item;
        return;
    }
}

template<typename Type>
void myList<Type>::Add(int Position, Type Item)
{
    if (Position == 1 || this->header == NULL) // 첫번 째위치 추가시 위의 함수를 이용
    {
        this->Add_First(Item);
        return;
    }
    Node<Type>* New = new Node<Type>;
    Node<Type>* Cur = this->header; //헤더는 움직이면 안됨!
    int pos = 1;
    while (pos > Position - 1)
    {
        Cur = Cur->Link;
        pos++;
    }
    New->VALUE = Item;
    New->Link = Cur->Link;
    Cur->Link = New;
}

template<typename Type>
Type myList<Type>::Delete(int Position)
{
    if (this->Is_Empty())
    {
        return 0;
    }
    int pos = 1;
    Type Return_Data;
    Node<Type>* Temp = NULL;
    Node<Type>* Cur = this->header;
    while (pos < Position)
    {
        if (pos == Position - 1)
            Temp = Cur;
        Cur = Cur->Link;
        pos++;
    }
    Temp->Link = Cur->Link;
    Return_Data = Cur->VALUE;
    delete(Cur);
    return Return_Data;
}

template<typename Type>
void myList<Type>::Replace(int Position, Type Item)
{
    int pos = 1;
    Node<Type>* Cur = this->header;
    while (pos < Position)
    {
        Cur = Cur->Link;
        pos++;
    }
    Cur->VALUE = Item;
}

template<typename Type>
Type myList<Type>::Find(Type Item)
{
    Node<Type>* Cur = this->header;
    int i = 1;
    while (Cur != NULL)
    {
        if (Cur->VALUE == Item)
        {
            return Cur;
        }
        i++;
        Cur = Cur->Link;
    }
    if (Cur == NULL)
        return nullptr;
}

template<typename Type>
void myList<Type>::Get_Entry(int Position)
{
    int pos = 1;
    Node<Type>* Cur = this->header;
    while (pos < Position)
    {
        Cur = Cur->Link;
        pos++;
    }
    std::cout << "In " << Position << "(th) node has this value : " << Cur->VALUE << std::endl;
}

template<typename Type>
void myList<Type>::Get_Length()
{
    Node<Type>* Cur = this->header;
    int i = 1;
    while (Cur != NULL)
    {
        i++;
        Cur = Cur->Link;
    }
    std::cout << "This List has " << i << " Nodes" << std::endl;
}

template<typename Type>
bool myList<Type>::Is_Empty()
{
    if (this->header == NULL)
    {
        std::cout << "This List is empty" << std::endl;
        return true;
    }
    std::cout << "This List is not empty" << std::endl;
    return false;
}

template<typename Type>
void myList<Type>::Printing()
{
    Node<Type>* Cur = this->header;
    std::cout << Cur->VALUE << " -> ";
    Cur = Cur->Link;
    while (Cur != NULL)
    {
        std::cout << Cur->VALUE << " -> ";
        Cur = Cur->Link;
    }
    std::cout << std::endl;

}