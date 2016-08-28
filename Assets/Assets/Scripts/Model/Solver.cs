using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;
public sealed class SolverConfiguration {
	public int resultLimit;
	public SolverConfiguration(int resultLimit) {
		this.resultLimit = resultLimit;
	}
}

public sealed class Solver {
	
	private SolverConfiguration config;
	private ScrabbleBoard board;
	private ScrabblePlayerMoveScoring scoring;
	public Solver(SolverConfiguration config, ScrabbleBoard board, ScrabblePlayerMoveScoring scoring) {
		this.config = config;
		this.board = board;
		this.scoring = scoring;
	}
	
	public List<PredictionResult> solve(List<Tile> tiles) {
	
		List<PredictionResult> resultSet = new List<PredictionResult>();
		for (int i = 0; i < board.dimension; i++) {
			
			for (int j = 0; j < board.dimension; j++) {
				
				Coordinate coordinate = new Coordinate(i, j);
				
				Prediction prediction = new Prediction(scoring);
				foreach (PredictionResult result in prediction.predict(board, tiles, coordinate)) {
					resultSet.Add (result);
				}
			}
		}
		
		return top(resultSet);



//
//
//		ThreadSafeList<PredictionResult> resultSet = new ThreadSafeList<PredictionResult>();
//
//		int numThreads = 4;
//
//		ManualResetEvent resetEvent1 = new ManualResetEvent(false);
//		int toProcess1 = numThreads;
//		new Thread(delegate()
//			{
//				for (int i = 0; i < board.dimension / 2; i++) {
//					
//					for (int j = 0; j < board.dimension / 2; j++) {
//						Coordinate coordinate = new Coordinate(i, j);
//						//						Debug.Log("hello, from " + Thread.CurrentThread.ManagedThreadId);
//						
//						Prediction prediction = new Prediction(scoring);
//						foreach (PredictionResult result in prediction.predict(board, tiles, coordinate)) {
//							resultSet.Add (result);
//						}
//						
//						if (Interlocked.Decrement(ref toProcess1) == 0)
//							resetEvent1.Set();
//						
//					}
//				}
//			}).Start();
//
//		ManualResetEvent resetEvent2 = new ManualResetEvent(false);
//		int toProcess2 = numThreads;
//		new Thread(delegate()
//			{
//				for (int i = 0; i < board.dimension / 2; i++) {
//
//					for (int j = board.dimension / 2; j < board.dimension; j++) {
//						Coordinate coordinate = new Coordinate(i, j);
//						//						Debug.Log("hello, from " + Thread.CurrentThread.ManagedThreadId);
//
//						Prediction prediction = new Prediction(scoring);
//						foreach (PredictionResult result in prediction.predict(board, tiles, coordinate)) {
//							resultSet.Add (result);
//						}
//
//						if (Interlocked.Decrement(ref toProcess2) == 0)
//							resetEvent2.Set();
//
//					}
//				}
//			}).Start();
//
//		ManualResetEvent resetEvent3 = new ManualResetEvent(false);
//		int toProcess3 = numThreads;
//		new Thread(delegate()
//			{
//				for (int i = board.dimension / 2; i < board.dimension; i++) {
//
//					for (int j = 0; j < board.dimension / 2; j++) {
//						Coordinate coordinate = new Coordinate(i, j);
//						//						Debug.Log("hello, from " + Thread.CurrentThread.ManagedThreadId);
//
//						Prediction prediction = new Prediction(scoring);
//						foreach (PredictionResult result in prediction.predict(board, tiles, coordinate)) {
//							resultSet.Add (result);
//						}
//
//						if (Interlocked.Decrement(ref toProcess3) == 0)
//							resetEvent3.Set();
//
//					}
//				}
//			}).Start();
//
//		ManualResetEvent resetEvent4 = new ManualResetEvent(false);
//		int toProcess4 = numThreads;
//		new Thread(delegate()
//			{
//				for (int i = board.dimension / 2; i < board.dimension; i++) {
//
//					for (int j = board.dimension / 2; j < board.dimension; j++) {
//						Coordinate coordinate = new Coordinate(i, j);
//						//						Debug.Log("hello, from " + Thread.CurrentThread.ManagedThreadId);
//
//						Prediction prediction = new Prediction(scoring);
//						foreach (PredictionResult result in prediction.predict(board, tiles, coordinate)) {
//							resultSet.Add (result);
//						}
//
//						if (Interlocked.Decrement(ref toProcess4) == 0)
//							resetEvent4.Set();
//
//					}
//				}
//			}).Start();
//		
//		resetEvent1.WaitOne();
//		resetEvent2.WaitOne();
//		resetEvent3.WaitOne();
//		resetEvent4.WaitOne();
////		Debug.Log("Finished.");
//
//		return top(resultSet);
	}
	private List<PredictionResult> top(List<PredictionResult> list) {
		list.Sort ();
		return list.GetRange(0, (list.Count < config.resultLimit) ? list.Count : config.resultLimit);
	}
	private List<PredictionResult> top(ThreadSafeList<PredictionResult> tslist) {
		List<PredictionResult> list = tslist.toList ();
		list.Sort ();
		return list.GetRange(0, (list.Count < config.resultLimit) ? list.Count : config.resultLimit);
	}
}


class ThreadSafeList<T> { 
	private List<T> _list = new List<T>();
	private object _sync = new object();
	public void Add(T value) {
		lock (_sync) {
			_list.Add(value);
		}
	}
	public List<T> toList() {
		return _list;
	}
}