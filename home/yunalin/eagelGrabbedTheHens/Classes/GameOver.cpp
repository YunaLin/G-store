#include "GameOver.h"
#include "MenuScene.h"
#include "Global.h"
#include "GameScene.h"
#include "network/HttpClient.h"
#include "json/rapidjson.h"
#include "json/document.h"

#define database UserDefault::getInstance()
USING_NS_CC;

using namespace cocos2d::network;

Scene* GameOver::createScene() {
	// 'scene' is an autorelease object
	auto scene = Scene::create();

	// 'layer' is an autorelease object
	auto layer = GameOver::create();

	// add layer as a child to scene
	scene->addChild(layer);

	// return the scene
	return scene;
}

bool GameOver::init() {
	if (!Layer::init()) {
		return false;
	}
	auto visibleSize = Director::getInstance()->getVisibleSize();
	auto visibleHeight = visibleSize.height;
	auto visibleWidth = visibleSize.width;
	Vec2 origin = Director::getInstance()->getVisibleOrigin();


	auto entry = Sprite::create("background.png");
	entry->setPosition(Vec2(visibleWidth / 2, visibleHeight / 2));
	this->addChild(entry, 0);

	auto background = Sprite::create("gameOver.png");
	background->setPosition(Vec2(origin.x + visibleSize.width / 2, origin.y + visibleSize.height / 3+200));
	this->addChild(background, 0);

	//����û���
	Label* username = Label::createWithTTF(database->getStringForKey("username"), "fonts/STXINWEI.TTF", 40);
	username->setPosition(110, visibleHeight - 40);
	username->setColor(Color3B(25, 25, 25));
	username->setAnchorPoint(Vec2(0.2f, 0.5f));
	this->addChild(username);
	//���Logout��ť
	auto label = Label::createWithTTF("Logout", "fonts/Marker Felt.ttf", 40);
	label->setColor(Color3B(25, 25, 25));
	auto logoutBtn = MenuItemLabel::create(label, CC_CALLBACK_1(GameOver::logoutCallback, this));
	Menu* logout = Menu::create(logoutBtn, NULL);
	logout->setPosition(visibleSize.width - 80, 50);
	this->addChild(logout);
	//���continue��ť
	label = Label::createWithTTF("Continue", "fonts/Marker Felt.ttf", 40);
	label->setColor(Color3B(25, 25, 25));
	auto continueBtn = MenuItemLabel::create(label, CC_CALLBACK_1(GameOver::continueCallback, this));
	Menu* next = Menu::create(continueBtn, NULL);
	next->setPosition(visibleSize.width - 250, 50);
	this->addChild(next);
	//����score
	string str = "Your score: ";
	char s[10];
	_itoa(database->getIntegerForKey("scoreInt"), s, 10);
	str += s;
	score = Label::createWithTTF(str, "fonts/STXINWEI.TTF", 40);
	score->setPosition(visibleWidth / 2 - 10, visibleHeight / 2);
	this->addChild(score);

	//������ť
	label = Label::createWithTTF("Rank", "fonts/Marker Felt.ttf", 40);
	label->setColor(Color3B(25, 25, 25));
	label->setPosition(visibleSize.width - 250, origin.y + visibleSize.height / 3 + 250);
	this->addChild(label,3);

	submitCallback(nullptr);

	rank_field = Label::createWithTTF("", "fonts/STXINWEI.TTF", 40);
	rank_field->setPosition(visibleWidth / 2 + 300, origin.y + visibleHeight - 200);
	rank_field->setColor(Color3B(25, 25, 25));
	rank_field->setContentSize(Size(visibleWidth / 4, visibleHeight / 5 * 3));
	rank_field->setAnchorPoint(Vec2(0.5, 1));
	this->addChild(rank_field,2);

	rankCallback(nullptr);

	return true;
}

// ����ύ��ť
void GameOver::submitCallback(Ref* sender) {
	HttpRequest* request = new HttpRequest();
	request->setRequestType(HttpRequest::Type::POST);
	request->setUrl("http://localhost:8080/submit");
	request->setResponseCallback(CC_CALLBACK_2(GameOver::onsubmitCompleted, this));
	vector<string> header;
	header.push_back("Cookie: GAMESESSIONID=" + Global::gameSessionId);
	request->setHeaders(header);
	char s[10];
	_itoa(database->getIntegerForKey("scoreInt"), s, 10);
	string postdata = "score=";
	postdata += s;
	request->setRequestData(postdata.c_str(), postdata.size());
	HttpClient::getInstance()->send(request);
	request->release();
}

//// ���������ť,����ѯǰ10��,Ĭ�ϲ�ǰʮ��,
void GameOver::rankCallback(Ref * sender) {
	HttpRequest* request = new HttpRequest();
	request->setRequestType(HttpRequest::Type::GET);
	string data = "top=10";
	request->setUrl(("http://localhost:8080/rank?" + data).c_str());
	request->setResponseCallback(CC_CALLBACK_2(GameOver::rankCompleted, this));
	vector<string> header;
	header.push_back("Cookie: GAMESESSIONID=" + Global::gameSessionId);
	request->setHeaders(header);
	request->setRequestData(data.c_str(), data.size());
	HttpClient::getInstance()->send(request);
	request->release();
}

// ���username,���ؿ�ʼ����
void GameOver::logoutCallback(Ref * sender) {
	database->setStringForKey("username", ""); 
	Director::getInstance()->replaceScene(TransitionCrossFade::create(0.3f, MenuScene::createScene()));
}

//������Ϸ���棬������Ϸ
void GameOver::continueCallback(Ref * pSender) {
	Director::getInstance()->replaceScene(TransitionCrossFade::create(0.4f, GameScene::createScene()));
}

// �ύ��Ӧ����,����ɹ��ύ���������
void GameOver::onsubmitCompleted(HttpClient* sender, HttpResponse* res) {
}

// ������Ӧ����,�ɹ�����ʾǰn������
void GameOver::rankCompleted(HttpClient* sender, HttpResponse* response) {
	if (response == nullptr) return;
	if (response->isSucceed()) {
		std::vector<char> *resHeader = response->getResponseHeader();
		std::vector<char> *resBody = response->getResponseData();
		string result1 = "";
		auto it = resHeader->begin();
		string ResHeader = "";
		for (it = resHeader->begin(); it != resHeader->end(); it++) {
			ResHeader += (*it);
		}
		string ResBody = "";
		for (it = resBody->begin(); it != resBody->end(); it++) {
			ResBody += (*it);
		}

		rapidjson::Document rankfile;
		rankfile.Parse(ResBody.c_str());
		if (!rankfile.HasParseError() && rankfile.HasMember("result") && rankfile["result"].GetBool()) {
			string rank = rankfile["info"].GetString();
			rank.erase(0, 1);
			for (int i = 0; i < rank.length(); i++)
				if (rank[i] == '|')
					rank[i] = '\n';
			rank_field->setString(rank);
		}
	}
}
